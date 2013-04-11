//
// EasePublisher.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using MonoDevelop.Ide;
using MonoDevelop.Core;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.IPhone;
using MonoDevelop.Projects;

namespace Apperian.Ease.Publisher
{
	//this class is a state machine for uploading apps
	public class EasePublisher
	{
		enum State {
			Idle,
			Authenticated,
			AppsListed,
			Updated,
			Created,
			Uploaded,
			Published,
			Done,
			Error = 100
		}

		State state;
		string token, appId, transactionId, fileUploadUrl, fileId;

		Project project;
		ProjectConfiguration configuration;
		string url, targetName, email, password, appName;
		IList<EaseApplication> listedApplications;
		Stream application;
		Exception error;
		Action<string> onAuthenticated, onSuccess;
		Action onError;

		AggregatedProgressMonitor monitor;
		public EasePublisher (Project project, ProjectConfiguration configuration,
		                      string url, string targetName, string email, string password, 
		                      string appName, Stream application, Action<string> onAuthenticated, Action<string> onSuccess, Action onError)
		{
			this.project = project;
			this.configuration = configuration;
			this.url = url;
			this.targetName = targetName;
			this.email = email;
			this.password = password;
			this.appName = appName;
			this.application = application;
			this.onAuthenticated = onAuthenticated;
			this.onSuccess = onSuccess;
			this.onError = onError;
		}

		public void Start ()
		{
			SetState (State.Idle);
		}

		void SetState (State newstate)
		{
			switch (newstate) {
			case State.Idle:
				Authenticate ();
				break;
			case State.Authenticated:
				GetApplicationList ();
				break;
			case State.AppsListed:
				var app = listedApplications.Where (ea => ea.Name == appName).FirstOrDefault ();
				if (app == null)
					Create ();
				else
					Update ();
				break;
			case State.Created:
			case State.Updated:
				Upload ();
				break;
			case State.Uploaded:
				Publish ();
				break;
			case State.Done:
				if (onSuccess != null)
					onSuccess (appName);
				break;
			case State.Error:
				ReportError ();
				break;
			}
		}

		void Authenticate ()
		{
#if DEBUG
			Console.WriteLine ("Authenticate.");
#endif
			try {
				monitor = new AggregatedProgressMonitor ();
				monitor.AddSlaveMonitor (IdeApp.Workbench.ProgressMonitors.GetOutputProgressMonitor (
					"Register to Apperian Ease",
					MonoDevelop.Ide.Gui.Stock.RunProgramIcon, false, true));
				monitor.AddSlaveMonitor (IdeApp.Workbench.ProgressMonitors.GetStatusProgressMonitor (
					"Register to Apperian Ease",
					MonoDevelop.Ide.Gui.Stock.RunProgramIcon, true, true, false));
				
				if (string.IsNullOrEmpty (url)) {
					monitor.ReportError ("A Publish URL is required to register a Publish Target", null);
					return;
				}
				
				if (string.IsNullOrEmpty (targetName)) {
					monitor.ReportError ("A Target Name is required to register a Publish Target", null);
					return;
				}
				
				if (string.IsNullOrEmpty (email)) {
					monitor.ReportError ("An Email is required to register a Publish Target", null);
					return;
				}
				
				if (string.IsNullOrEmpty (password)) {
					monitor.ReportError ("A Password is required to register a Publish Target", null);
					return;
				}

				Action<string> onAuthenticatedAction = (t) => {
					token = t; 
					if (onAuthenticated != null) 
						onAuthenticated (targetName);
					SetState (State.Authenticated);
				};

				var request = new AuthenticateRequest (onAuthenticatedAction, 
					(e) => {error = e; SetState (State.Error); }
				);
				request.Authenticate (url, email, password);
			} catch (Exception ex) {
				// If we leak any exceptions out of this method, the process will die, so be a bit paranoid about it.
				try {
					monitor.ReportError ("Unhandled error while registering", null);
					LoggingService.LogError ("Unhandled exception while registering", ex);
				} catch {
				}
			} finally {
				monitor.Dispose ();
			}
		}

		void GetApplicationList ()
		{
#if DEBUG
			Console.WriteLine ("GetApplication List.");
#endif
			try {
				monitor = new AggregatedProgressMonitor ();
				monitor.AddSlaveMonitor (IdeApp.Workbench.ProgressMonitors.GetOutputProgressMonitor (
					"Get List of Apperian Ease Applications",
					MonoDevelop.Ide.Gui.Stock.RunProgramIcon, false, true));
				monitor.AddSlaveMonitor (IdeApp.Workbench.ProgressMonitors.GetStatusProgressMonitor (
					"Get List of Apperian Ease Applications",
					MonoDevelop.Ide.Gui.Stock.RunProgramIcon, true, true, false));
				
				if (string.IsNullOrEmpty (token)) {
					monitor.ReportError ("An authentication token is required", null);
					return;
				}

				Action<IList<EaseApplication>> onGetListAction = (l) => {
					listedApplications = l;
					SetState (State.AppsListed);
				};
				
				var request = new GetListRequest (onGetListAction, 
				                                       (e) => {error = e; SetState (State.Error); }
				);
				request.GetList (url, token);
			} catch (Exception ex) {
				// If we leak any exceptions out of this method, the process will die, so be a bit paranoid about it.
				try {
					monitor.ReportError ("Unhandled error while getting app list", null);
					LoggingService.LogError ("Unhandled exception while getting app list", ex);
				} catch {
				}
			} finally {
				monitor.Dispose ();
			}
		}

		void Create ()
		{
#if DEBUG
			Console.WriteLine ("Create.");
#endif
			try {
				monitor = new AggregatedProgressMonitor ();
				monitor.AddSlaveMonitor (IdeApp.Workbench.ProgressMonitors.GetOutputProgressMonitor (
					"Create Apperian EASE Application",
					MonoDevelop.Ide.Gui.Stock.RunProgramIcon, false, true));
				monitor.AddSlaveMonitor (IdeApp.Workbench.ProgressMonitors.GetStatusProgressMonitor (
					"Create Apperian EASE Application",
					MonoDevelop.Ide.Gui.Stock.RunProgramIcon, true, true, false));
				
				if (string.IsNullOrEmpty (token)) {
					monitor.ReportError ("An authentication token is required", null);
					return;
				}
				
				Action<Transaction> onCreateAction = (t) => {
					transactionId = t.Id;
					fileUploadUrl = t.FileUploadUrl;
					SetState (State.Created);
				};
				
				var request = new CreateRequest (onCreateAction, 
				                                  (e) => {error = e; SetState (State.Error); }
				);
				request.Create (url, token);
			} catch (Exception ex) {
				// If we leak any exceptions out of this method, the process will die, so be a bit paranoid about it.
				try {
					monitor.ReportError ("Unhandled error while creating", null);
					LoggingService.LogError ("Unhandled exception while creating", ex);
				} catch {
				}
			} finally {
				monitor.Dispose ();
			}
		}

		void Update ()
		{
#if DEBUG
			Console.WriteLine ("Update.");
#endif
			try {
				monitor = new AggregatedProgressMonitor ();
				monitor.AddSlaveMonitor (IdeApp.Workbench.ProgressMonitors.GetOutputProgressMonitor (
					"Update Apperian EASE Application",
					MonoDevelop.Ide.Gui.Stock.RunProgramIcon, false, true));
				monitor.AddSlaveMonitor (IdeApp.Workbench.ProgressMonitors.GetStatusProgressMonitor (
					"Update Apperian EASE Application",
					MonoDevelop.Ide.Gui.Stock.RunProgramIcon, true, true, false));
				
				if (string.IsNullOrEmpty (token)) {
					monitor.ReportError ("An authentication token is required", null);
					return;
				}

				if (string.IsNullOrEmpty (appId)) {
					monitor.ReportError ("An app ID is required", null);
					return;
				}
				Action<Transaction> onUpdateAction = (t) => {
					transactionId = t.Id;
					fileUploadUrl = t.FileUploadUrl;
					SetState (State.Updated);
				};
				
				var request = new UpdateRequest (onUpdateAction, 
				                                 (e) => {error = e; SetState (State.Error); }
				);
				request.Update (url, appId, token);
			} catch (Exception ex) {
				// If we leak any exceptions out of this method, the process will die, so be a bit paranoid about it.
				try {
					monitor.ReportError ("Unhandled error while updating", null);
					LoggingService.LogError ("Unhandled exception while updating", ex);
				} catch {
				}
			} finally {
				monitor.Dispose ();
			}
		}

		void Upload ()
		{
#if DEBUG
			Console.WriteLine ("Upload.");
#endif
			try {
				monitor = new AggregatedProgressMonitor ();
				monitor.AddSlaveMonitor (IdeApp.Workbench.ProgressMonitors.GetOutputProgressMonitor (
					"Upload Apperian EASE Application",
					MonoDevelop.Ide.Gui.Stock.RunProgramIcon, false, true));
				monitor.AddSlaveMonitor (IdeApp.Workbench.ProgressMonitors.GetStatusProgressMonitor (
					"Upload Apperian EASE Application",
					MonoDevelop.Ide.Gui.Stock.RunProgramIcon, true, true, false));
				
				if (string.IsNullOrEmpty (fileUploadUrl)) {
					monitor.ReportError ("An file upload URL is required", null);
					return;
				}
				
				if (string.IsNullOrEmpty (transactionId)) {
					monitor.ReportError ("A transaction ID is required", null);
					return;
				}

				Action<string> onUploadAction = (r) => {
					fileId = r;
					SetState (State.Uploaded);
				};
				
				var request = new UploadRequest (onUploadAction, 
				                                 (e) => {error = e; SetState (State.Error); }
				);
				request.Upload (fileUploadUrl, transactionId, GetIpaFilename (project as IPhoneProject, configuration as IPhoneProjectConfiguration));
			} catch (Exception ex) {
				// If we leak any exceptions out of this method, the process will die, so be a bit paranoid about it.
				try {
					monitor.ReportError ("Unhandled error while uploading", null);
					LoggingService.LogError ("Unhandled exception while uploading", ex);
				} catch {
				}
			} finally {
				monitor.Dispose ();
			}
		}

		void Publish ()
		{
			//#if DEBUG
			//Console.WriteLine ("Publish.");
			//#endif
			//try {
			//	monitor = new AggregatedProgressMonitor ();
			//	monitor.AddSlaveMonitor (IdeApp.Workbench.ProgressMonitors.GetOutputProgressMonitor (
			//		"Publish to Apperian EASE Application",
			//		MonoDevelop.Ide.Gui.Stock.RunProgramIcon, false, true));
			//	monitor.AddSlaveMonitor (IdeApp.Workbench.ProgressMonitors.GetStatusProgressMonitor (
			//		"Publish to Apperian EASE Application",
			//		MonoDevelop.Ide.Gui.Stock.RunProgramIcon, true, true, false));
			//	
			//	if (string.IsNullOrEmpty (token)) {
			//		monitor.ReportError ("An authentication token is required", null);
			//		return;
			//	}
			//
			//	if (string.IsNullOrEmpty (transactionId)) {
			//		monitor.ReportError ("A trnasaction ID is required", null);
			//		return;
			//	}
			//
			//	if (string.IsNullOrEmpty (fileId)) {
			//		monitor.ReportError ("A file ID is required", null);
			//		return;
			//	}
			//
			//	if (metadata == null) {
			//		monitor.ReportError ("metadata is required", null);
			//		return;
			//	}
			//
			//	Action<string> onPublishAction = (t) => {
			//		appId = t;
			//		SetState (State.Done);
			//	};
			//	
			//	var request = new PublishRequest (onPublishAction, 
			//	                                 (e) => {error = e; SetState (State.Error); }
			//	);
			//	request.Publish (url, appId, token);
			//} catch (Exception ex) {
			//	// If we leak any exceptions out of this method, the process will die, so be a bit paranoid about it.
			//	try {
			//		monitor.ReportError ("Unhandled error while updating", null);
			//		LoggingService.LogError ("Unhandled exception while updating", ex);
			//	} catch {
			//	}
			//} finally {
			//	monitor.Dispose ();
			//}
		}

		static string GetIpaFilename (IPhoneProject proj, IPhoneProjectConfiguration conf)
		{
			MonoDevelop.MacDev.PlistEditor.PDictionary a;
			var plist = proj.GetInfoPlistDocument ();
			var name = conf.IpaPackageName;
			if (string.IsNullOrEmpty (name)) {
				name = conf.CompiledOutputName.FileNameWithoutExtension;
				string version = null;
				//var version = plist != null ? plist.GetCFBundleVersion () : null;
				if (!string.IsNullOrEmpty (version))
					name += "-" + version;
			}
			if (!name.EndsWith (".ipa", StringComparison.OrdinalIgnoreCase))
				name += ".ipa";
			return Path.Combine (conf.OutputDirectory, name);
		}

		void ReportError ()
		{
#if DEBUG
			Console.WriteLine ("Report Error.");
#endif
			monitor.ReportError (error.Message, error);
			if (onError != null)
				onError ();
		}
	}
}


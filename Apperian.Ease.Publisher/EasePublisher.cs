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
			Error = -1,
			Idle = 0,
			Authenticated,
			AppsListed,
			Updated,
			Created,
			Uploaded,
			Published,
			Done,
		}

		State state = State.Idle;
		State stopAfter = State.Done;

		string token, appId, transactionId, fileUploadUrl, fileId;

		public static Project Project { get { return PublishHandler.GetActiveExecutableIPhoneProject (); } }

		public ProjectConfiguration Configuration { get; private set;}
		string url, targetName, email, password;
		public EaseMetadata Metadata { get; private set;}

		public string ApplicationName {
			get {
				var iphoneconfig = Configuration as IPhoneProjectConfiguration;
				if (iphoneconfig != null && !string.IsNullOrEmpty(iphoneconfig.IpaPackageName))
					return iphoneconfig.IpaPackageName;
				return Project.Name;
			}
		}
		Exception error;
		Action<string> onAuthenticated;
		Action onSuccess;
		Action onError;

		AggregatedProgressMonitor monitor;
		public EasePublisher (string url, string targetName, string email, string password, EaseMetadata metadata)
		{
			this.Configuration = (IPhoneProjectConfiguration) Project.GetConfiguration (IdeApp.Workspace.ActiveConfiguration);

			this.url = url;
			this.targetName = targetName;
			this.email = email;
			this.password = password;
			this.Metadata = metadata;
		}

		public void Start (Action<string> authenticated, Action success, Action error)
		{	
			this.onAuthenticated = authenticated;
			this.onSuccess = success;
			this.onError = error;
			try {
				monitor = new AggregatedProgressMonitor ();
				monitor.AddSlaveMonitor (IdeApp.Workbench.ProgressMonitors.GetOutputProgressMonitor (
					"Publishing to Apperian Ease",
					MonoDevelop.Ide.Gui.Stock.RunProgramIcon, false, true));
				monitor.AddSlaveMonitor (IdeApp.Workbench.ProgressMonitors.GetStatusProgressMonitor (
					"Publishing to Apperian Ease",
					MonoDevelop.Ide.Gui.Stock.RunProgramIcon, true, true, false));

				SetState (State.Idle);
			} catch (Exception ex) {
				// If we leak any exceptions out of this method, the process will die, so be a bit paranoid about it.
				try {
					monitor.ReportError ("Unhandled error while publishing", null);
					LoggingService.LogError ("Unhandled exception while publishing", ex);
				} catch {
				}
			} finally {
				monitor.Dispose ();
			}
		}

		public void GetList (Action<string> authenticated, Action success, Action error)
		{
			stopAfter = State.AppsListed;
			Start (authenticated, success, error);
		}

		void SetState (State newstate)
		{
			if ((int)newstate >= (int)stopAfter)
				newstate = State.Done;

			switch (newstate) {
			case State.Idle:
				Authenticate ();
				break;
			case State.Authenticated:
				GetApplicationList ();
				break;
			case State.AppsListed:
				//var app = listedApplications.Where (ea => ea.Name == ApplicationName).FirstOrDefault ();
				if (appId == null)
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
				monitor.Log.WriteLine ("Operation completed successfully");
				if (onSuccess != null)
					onSuccess ();
				break;
			case State.Error:
				ReportError ();
				break;
			}
		}

		void Authenticate ()
		{
			monitor.Log.Write ("Authenticating...");
#if DEBUG
			Console.WriteLine ("Authenticate.");
#endif
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
				monitor.Log.WriteLine ("done");
				token = t; 
				if (onAuthenticated != null) 
					onAuthenticated (targetName);
				SetState (State.Authenticated);
			};

			var request = new AuthenticateRequest (onAuthenticatedAction, 
				(e) => {
				monitor.Log.WriteLine ("FAILED");
				error = e; SetState (State.Error); }
			);
			request.Authenticate (url, email, password);
		}

		void GetApplicationList ()
		{
			monitor.Log.Write ("Getting application list...");
#if DEBUG
			Console.WriteLine ("GetApplication List.");
#endif
			if (string.IsNullOrEmpty (token)) {
				monitor.ReportError ("An authentication token is required", null);
				return;
			}

			Action<IList<EaseApplication>> onGetListAction = (listedApps) => {
				monitor.Log.WriteLine ("done");
				var projType = (Project is IPhoneProject) ? "iOS App (IPA File)" : "Android App (APK File)";
				var applicationToUpdate = listedApps.Where(a => a.Name == ApplicationName && a.ApplicationType == projType).FirstOrDefault ();
				if (applicationToUpdate != null) {
					appId = applicationToUpdate.Id;
					if (stopAfter == State.AppsListed)
						Metadata = applicationToUpdate.Metadata;
				}
				SetState (State.AppsListed);
			};
			
			var request = new GetListRequest (onGetListAction, 
			                                       (e) => {
				monitor.Log.WriteLine ("FAILED");
				error = e; SetState (State.Error); }
			);
			request.GetList (url, token);
		}

		void Create ()
		{
			monitor.Log.Write ("Creating application...");
#if DEBUG
			Console.WriteLine ("Create.");
#endif
			if (string.IsNullOrEmpty (token)) {
				monitor.ReportError ("An authentication token is required", null);
				return;
			}
			
			Action<Transaction> onCreateAction = (t) => {
				monitor.Log.WriteLine ("done");
				transactionId = t.Id;
				fileUploadUrl = t.FileUploadUrl;
				SetState (State.Created);
			};
			
			var request = new CreateRequest (onCreateAction, 
			                                  (e) => {
				monitor.Log.WriteLine ("FAILED");
				error = e; SetState (State.Error); }
			);
			request.Create (url, token);
		}

		void Update ()
		{
			monitor.Log.Write ("Updating application...");
#if DEBUG
			Console.WriteLine ("Update.");
#endif
			if (string.IsNullOrEmpty (token)) {
				monitor.ReportError ("An authentication token is required", null);
				return;
			}

			if (string.IsNullOrEmpty (appId)) {
				monitor.ReportError ("An app ID is required", null);
				return;
			}
			Action<Transaction> onUpdateAction = (t) => {
				monitor.Log.WriteLine ("done");
				transactionId = t.Id;
				fileUploadUrl = t.FileUploadUrl;
				SetState (State.Updated);
			};
			
			var request = new UpdateRequest (onUpdateAction, 
			                                 (e) => {
				monitor.Log.WriteLine ("FAILED");
				error = e; 
				SetState (State.Error); 
			}
			);
			request.Update (url, appId, token);
		}

		void Upload ()
		{
			monitor.Log.Write ("Uploading application...");
#if DEBUG
			Console.WriteLine ("Upload.");
#endif
			if (string.IsNullOrEmpty (fileUploadUrl)) {
				monitor.ReportError ("An file upload URL is required", null);
				return;
			}
			
			if (string.IsNullOrEmpty (transactionId)) {
				monitor.ReportError ("A transaction ID is required", null);
				return;
			}

			Action<string> onUploadAction = (r) => {
				monitor.Log.WriteLine ("done");
				fileId = r;
				SetState (State.Uploaded);
			};
			
			var request = new UploadRequest (onUploadAction, 
			                                 (e) => {
				monitor.Log.WriteLine ("FAILED");
				error = e; 
				SetState (State.Error); 
			});
			request.Upload (fileUploadUrl, transactionId, GetIpaFilename (Project as IPhoneProject, Configuration as IPhoneProjectConfiguration));
		}

		void Publish ()
		{
			monitor.Log.Write ("Publishing application...");
#if DEBUG
			Console.WriteLine ("Publish.");
#endif
			if (string.IsNullOrEmpty (token)) {
				monitor.ReportError ("An authentication token is required", null);
				return;
			}
			
			if (string.IsNullOrEmpty (transactionId)) {
				monitor.ReportError ("A trnasaction ID is required", null);
				return;
			}
			
			if (string.IsNullOrEmpty (fileId)) {
				monitor.ReportError ("A file ID is required", null);
				return;
			}
			
			if (Metadata == null) {
				monitor.ReportError ("metadata is required", null);
				return;
			}
			
			Action<string> onPublishAction = (t) => {
				monitor.Log.WriteLine ("done");
				appId = t;
				SetState (State.Done);
			};
			
			var request = new PublishRequest (onPublishAction, 
			                                 (e) => {
				monitor.Log.WriteLine ("FAILED");
				error = e; 
				SetState (State.Error); 
			});
			request.Publish (url, token, transactionId, fileId, Metadata);
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


//
// EasePublisher.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;
using System.Linq;

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

		State stopAfter = State.Done;

		string token, appId, transactionId, fileUploadUrl, fileId;

		DotNetProject Project { get;  set;}
		DotNetProjectConfiguration Configuration { get;  set;}

		string url, targetName, email, password;
		public EaseMetadata Metadata { get; private set;}

		Exception error;
		Action<string> onAuthenticated;
		Action onSuccess;
		Action onError;

		AggregatedProgressMonitor monitor;
		public EasePublisher (DotNetProject project, DotNetProjectConfiguration configuration, string url, string targetName, string email, string password, EaseMetadata metadata)
		{
			Project = project;
			Configuration = configuration;

			this.url = url;
			this.targetName = targetName;
			this.email = email;
			this.password = password;
			this.Metadata = metadata;
		}

		public void Start (Action<string> authenticated, Action success, Action error)
		{	
			onAuthenticated = authenticated;
			onSuccess = success;
			onError = error;
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

		public void GetList (Action<string> authenticated, Action success, Action err)
		{
			stopAfter = State.AppsListed;
			Start (authenticated, success, err);
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

			Action<AuthenticateResult> onAuthenticatedAction = (r) => {
				monitor.Log.WriteLine ("done");
				token = r.Result.Token; 
				if (onAuthenticated != null) 
					onAuthenticated (targetName);
				SetState (State.Authenticated);
			};

			var request = new AuthenticateRequest (url, email, password, onAuthenticatedAction, 
				(e) => {
				monitor.Log.WriteLine ("FAILED");
				error = e; SetState (State.Error); }
			);
			request.Send ();
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

			Action<GetListResult> onGetListAction = (result) => {
				monitor.Log.WriteLine ("done");

				var projType = (Project is IPhoneProject) ? "iOS App (IPA File)" : "Android App (APK File)";
				var app = result.Result.Applications.Where(a => a.Name == Project.GetApplicationName () && a.ApplicationType == projType).FirstOrDefault ();
				if (app != null) {
					appId = app.ID;
					if (stopAfter == State.AppsListed)
					Metadata = new EaseMetadata {
						Author = app.Author,
						LongDescription = app.LongDescription,
						ShortDescription = app.ShortDescription,
						Version = app.Version,
						VersionNotes = app.VersionNotes,
						Name = app.Name,
					};
				}
				SetState (State.AppsListed);
			};
			
			var request = new GetListRequest (url, token, onGetListAction, 
			                                       (e) => {
				monitor.Log.WriteLine ("FAILED");
				error = e; SetState (State.Error); }
			);
			request.Send ();
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
			
			Action<CreateOrUpdateResult> onCreateAction = (t) => {
				monitor.Log.WriteLine ("done");
				transactionId = t.Result.TransactionID;
				fileUploadUrl = t.Result.FileUploadURL;
				SetState (State.Created);
			};
			
			var request = new CreateRequest (url, token, onCreateAction, 
			                                  (e) => {
				monitor.Log.WriteLine ("FAILED");
				error = e; SetState (State.Error); }
			);
			request.Send ();
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
			Action<CreateOrUpdateResult> onUpdateAction = (t) => {
				monitor.Log.WriteLine ("done");
				transactionId = t.Result.TransactionID;
				fileUploadUrl = t.Result.FileUploadURL;
				SetState (State.Updated);
			};
			
			var request = new UpdateRequest (url, appId, token, onUpdateAction, 
			                                 (e) => {
				monitor.Log.WriteLine ("FAILED");
				error = e; 
				SetState (State.Error); 
			}
			);
			request.Send ();
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

			Action<UploadResult> onUploadAction = (r) => {
				monitor.Log.WriteLine ("done");
				fileId = r.FileId;
				SetState (State.Uploaded);
			};
			
			var request = new UploadRequest (fileUploadUrl, transactionId, Project.GetBuildArtifact(Configuration), onUploadAction, 
			                                 (e) => {
				monitor.Log.WriteLine ("FAILED");
				error = e; 
				SetState (State.Error); 
			});
			request.Send ();
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
			
			Action<PublishResult> onPublishAction = (t) => {
				monitor.Log.WriteLine ("done");
				appId = t.Result.AppId;
				SetState (State.Done);
			};
			
			var request = new PublishRequest (url, token, transactionId, fileId, Metadata, onPublishAction, 
			                                 (e) => {
				monitor.Log.WriteLine ("FAILED");
				error = e; 
				SetState (State.Error); 
			});
			request.Send ();
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


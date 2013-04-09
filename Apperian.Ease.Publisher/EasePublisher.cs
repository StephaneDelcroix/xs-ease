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

using MonoDevelop.Ide;
using MonoDevelop.Core;
using MonoDevelop.Core.ProgressMonitoring;

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

		string url, targetName, email, password, appName;
		Stream application;
		Exception error;
		Action<string> onAuthenticated, onSuccess;
		Action onError;

		AggregatedProgressMonitor monitor;
		public EasePublisher (string url, string targetName, string email, string password, 
		                      string appName, Stream application, Action<string> onAuthenticated, Action<string> onSuccess, Action onError)
		{
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

		public void Publish ()
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
				break;
			case State.Error:
				ReportError ();
				break;
			}
		}

		void Authenticate ()
		{
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

		void ReportError ()
		{
			monitor.ReportError (error.Message, error);
			onError ();
		}
	}
}


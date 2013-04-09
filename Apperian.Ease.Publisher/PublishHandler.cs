//
// PublishHandler.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;
using MonoDevelop.Components.Commands;

using MonoDevelop.Ide;
using MonoDevelop.Core;
using MonoDevelop.Core.ProgressMonitoring;
using MonoDevelop.IPhone;

using Gtk;

namespace Apperian.Ease.Publisher
{
	public class PublishHandler : CommandHandler
	{
		protected override void Run ()
		{
			var project = IdeApp.ProjectOperations.CurrentSelectedProject as IPhoneProject;

#if DEBUG
			Console.WriteLine ("Run");
			Console.WriteLine (project.Name);
#endif
			var dialog = new PublisherDialog ();
			dialog.ShowAll ();
			if ((ResponseType)dialog.Run () == ResponseType.Ok) {
				IdeApp.ProjectOperations.Build (IdeApp.ProjectOperations.CurrentSelectedProject).Completed += delegate(IAsyncOperation op) {
					if (!op.Success) {
						MessageService.ShowError (
							"Cannot publish to Apperian EASE",
							"Project did not build successfully");
						return;
					}
					var publisher = new EasePublisher (dialog.TargetUrl, dialog.TargetName, dialog.TargetEmail, dialog.TargetPassword, null, null, null, null, null);

					var t = new System.Threading.Thread (publisher.Publish) {
						IsBackground = true,
						Name = "Publish to Apperian EASE",
					};
					t.Start ();
				};
			}
			dialog.Hide ();
		}

		protected override void Update (CommandInfo info)
		{
#if DEBUG
			Console.WriteLine ("Update");
#endif
			info.Enabled = IdeApp.ProjectOperations.CurrentSelectedProject != null && IdeApp.ProjectOperations.CurrentSelectedProject is IPhoneProject;
		}
	}
}


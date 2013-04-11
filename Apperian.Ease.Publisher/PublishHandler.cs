//
// PublishHandler.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
// Contains code from Xamarin, Inc. with authorization.
//
using System;
using MonoDevelop.Components.Commands;

using MonoDevelop.Ide;
using MonoDevelop.Core;
using MonoDevelop.IPhone;
using MonoDevelop.Projects;

using Gtk;

namespace Apperian.Ease.Publisher
{
	public class PublishHandler : CommandHandler
	{
		public static IPhoneProject GetActiveExecutableIPhoneProject ()
		{
			IPhoneProject phoneProject = IdeApp.ProjectOperations.CurrentSelectedProject as IPhoneProject;

			if (phoneProject != null && phoneProject.CompileTarget == CompileTarget.Exe)
				return phoneProject;

			Solution currentSelectedSolution = IdeApp.ProjectOperations.CurrentSelectedSolution;

			if (currentSelectedSolution != null) {
				phoneProject = (currentSelectedSolution.StartupItem as IPhoneProject);

				if (phoneProject != null && phoneProject.CompileTarget == CompileTarget.Exe)
					return phoneProject;
			}

			return null;
		}

		protected override void Run ()
		{
#if DEBUG
			Console.WriteLine ("Run");
#endif
			var proj = GetActiveExecutableIPhoneProject ();
			var conf = (IPhoneProjectConfiguration) proj.GetConfiguration (IdeApp.Workspace.ActiveConfiguration);

			//Enable IPA
			if (!conf.BuildIpa) {
				QuestionMessage qm = new QuestionMessage ();
				qm.Buttons.Add (AlertButton.Yes);
				qm.Buttons.Add (AlertButton.No);
				qm.Text = GettextCatalog.GetString ("Enable ad-hoc/enterprise packaging");
				qm.SecondaryText = 
					"You must enable ad-hoc/enterprise packaging (IPA support)" +
					" if you want to publish your application using Apperian. " +
					"Do you want to enable ad-hoc/enterprise packaging?";
				var response = MessageService.AskQuestion (qm);
				if (response == AlertButton.No)
					return;
				conf.BuildIpa = true;
				IdeApp.ProjectOperations.Save (proj);
			}


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
					var publisher = new EasePublisher (proj, conf, dialog.TargetUrl, dialog.TargetName, dialog.TargetEmail, dialog.TargetPassword, null, null, OnAuthenticated, null, null);

					var t = new System.Threading.Thread (publisher.Start) {
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
			info.Visible = IsVisibleIphone () || IsVisibleAndroid ();
			info.Enabled = IsEnabledIPhone () || IsEnabledAndroid ();
		}

		bool IsVisibleIphone ()
		{
			var proj = GetActiveExecutableIPhoneProject ();
			return proj != null;
		}

		bool IsVisibleAndroid ()
		{
			return false;
		}

		bool IsEnabledIPhone ()
		{
			var proj = GetActiveExecutableIPhoneProject ();
			if (proj == null)
				return false;
			var conf = (IPhoneProjectConfiguration) proj.GetConfiguration (IdeApp.Workspace.ActiveConfiguration);
			return conf.IsDevicePlatform;
		}

		bool IsEnabledAndroid ()
		{
			return false;
		}

		void OnAuthenticated (string target)
		{
			//TODO: save the target if it's not done already
		}
	}
}


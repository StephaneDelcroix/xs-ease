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
using System.Threading;

using Gtk;

using MonoDevelop.Ide;
using MonoDevelop.Core;
using MonoDevelop.IPhone;
using MonoDevelop.MonoDroid;
using MonoDevelop.Projects;
using MonoDevelop.Components.Commands;

namespace Apperian.Ease.Publisher
{
	public class PublishHandler : CommandHandler
	{
		protected override void Run ()
		{
#if DEBUG
			Console.WriteLine ("Run");
#endif
			var proj = GetActiveMobileProject ();
			var iphoneproject = proj as IPhoneProject;
			var androidproject = proj as MonoDroidProject;

			if (iphoneproject != null) {
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
			}

			if (androidproject != null) {
				//Add Application Manifest
				if (androidproject.AndroidManifest == FilePath.Null || androidproject.AndroidManifest == FilePath.Empty) {
					QuestionMessage qm = new QuestionMessage ();
					qm.Buttons.Add (AlertButton.Yes);
					qm.Buttons.Add (AlertButton.No);
					qm.Text = GettextCatalog.GetString ("Add Android manifest");
					qm.SecondaryText = 
						"The project has no Anbdroid manifest." +
							"Do you want to add one ?";
					var response = MessageService.AskQuestion (qm);
					if (response == AlertButton.No)
						return;
					androidproject.AddManifest();
					IdeApp.ProjectOperations.Save (proj);
				}
			}

			var dialog = new PublisherDialog ();
			dialog.ShowAll ();
			if ((ResponseType)dialog.Run () == ResponseType.Ok) {

				OperationHandler onbuild = 
				(IAsyncOperation op) => {
					if (!op.Success) {
						MessageService.ShowError (
							"Cannot publish to Apperian EASE",
							"Project did not build successfully");
						return;
					}
					var publisher = new EasePublisher (proj, (DotNetProjectConfiguration)proj.GetConfiguration (IdeApp.Workspace.ActiveConfiguration), dialog.TargetUrl, dialog.TargetName, dialog.TargetEmail, dialog.TargetPassword, dialog.Metadata);

					var t = new Thread (()=>{publisher.Start(null, null, null);}) {
						IsBackground = true,
						Name = "Publish to Apperian EASE",
					};
					t.Start (); 
				};
				if (androidproject != null)
					androidproject.PackageForAndroid (IdeApp.Workspace.ActiveConfiguration).Completed += onbuild;
				else
					IdeApp.ProjectOperations.Build (IdeApp.ProjectOperations.CurrentSelectedProject).Completed += onbuild;
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
			var proj = GetActiveMobileProject() as IPhoneProject;
			return proj != null;
		}

		bool IsVisibleAndroid ()
		{
			var proj = GetActiveMobileProject() as MonoDroidProject;
			return proj != null;
		}

		bool IsEnabledIPhone ()
		{
			var proj = GetActiveMobileProject() as IPhoneProject;
			if (proj == null)
				return false;
			var conf = (IPhoneProjectConfiguration) proj.GetConfiguration (IdeApp.Workspace.ActiveConfiguration);
			return conf.IsDevicePlatform;
		}

		bool IsEnabledAndroid ()
		{
			var proj = GetActiveMobileProject() as MonoDroidProject;
			return proj != null;
		}

		public static DotNetProject GetActiveMobileProject ()
		{
			var project = IdeApp.ProjectOperations.CurrentSelectedProject as DotNetProject;
			if (project != null) {
				if (project is IPhoneProject && project.CompileTarget == CompileTarget.Exe)
					return project;
				if (project is MonoDroidProject)
					return project;
			}

			var solution = IdeApp.ProjectOperations.CurrentSelectedSolution;
			if (solution == null)
				return null;

			project = solution.StartupItem as DotNetProject;
			if (project != null) {
				if (project is IPhoneProject && project.CompileTarget == CompileTarget.Exe)
					return project;
				if (project is MonoDroidProject)
					return project;
			}

			return null;
		}
	}
}
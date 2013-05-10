//
// PublisherDialog.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using MonoDevelop.Ide;
using MonoDevelop.Projects;

using Gtk;

namespace Apperian.Ease.Publisher
{
	public partial class PublisherDialog : Dialog
	{
		RegisterDialog registerdialog;
		List<Target> targets;
		EasePublisher publisher;
		public PublisherDialog ()
		{
			this.Build ();

			LoadTargets ();
			FillCombo ();

			nameEntry.Sensitive = authorEntry.Sensitive = versionEntry.Sensitive = descriptionEntry.Sensitive = 
				lonDescriptionEntry.Sensitive = versionNotesEntry.Sensitive = false;

			nameEntry.Text = PublishHandler.GetActiveMobileProject ().GetApplicationName ();
			authorEntry.Text = PublishHandler.GetActiveMobileProject ().AuthorInformation.Name;
			versionEntry.Text = PublishHandler.GetActiveMobileProject ().Version;

			buttonRegister.Clicked += OnRegisterClicked;
			comboboxTargets.Changed += OnTargetChanged;
			buttonPublish.Sensitive = false;
		}

		public string TargetName { get { return targets[comboboxTargets.Active].Name; }}
		public string TargetUrl { get { return targets[comboboxTargets.Active].Url; }}
		public string TargetEmail { get { return targets[comboboxTargets.Active].Email; }}
		public string TargetPassword { get { return targets[comboboxTargets.Active].Password; }}
		public EaseMetadata Metadata { get { return new EaseMetadata {
					Author = authorEntry.Text,
					LongDescription = lonDescriptionEntry.Buffer.Text.TrimAt (1000),
					Name = nameEntry.Text,
					ShortDescription = descriptionEntry.Text,
					Version = versionEntry.Text,
					VersionNotes = versionNotesEntry.Buffer.Text.TrimAt (1500),
				};
			}
		}

		string settingsKey = "ApperianEaseSettings";

		void LoadTargets ()
		{
			try {
				var prefs = PublishHandler.GetActiveMobileProject ().UserProperties.GetValue<string> (settingsKey);
				var settings = JsonConvert.DeserializeObject<TargetsSettings> (prefs);
				targets = settings.Targets ?? new List<Target> ();
			} catch (Exception e) {
				Console.WriteLine ("Failed to load Apperian EASE settings. {0}", e);
				targets = new List<Target> ();
			}
		}

		void SaveTargets ()
		{
			try {
				var settings = new TargetsSettings {
					Targets = targets,
				};

				var prefs = JsonConvert.SerializeObject (settings);
				Console.WriteLine (settings);
				PublishHandler.GetActiveMobileProject ().UserProperties.SetValue<string> (settingsKey, prefs);
			} catch (Exception e) {
				Console.WriteLine ("failed to save Apperian EASE settings. {0}", e);
			}
		}

		void FillCombo ()
		{
			comboboxTargets.Clear ();
			var cell = new CellRendererText ();
			comboboxTargets.PackStart (cell, false);
			comboboxTargets.AddAttribute (cell, "text", 0);
			var store = new ListStore(typeof (string));

			foreach (var item in targets) {
				Console.WriteLine (item);
				Console.WriteLine (item.Name);
				store.AppendValues (item.Name);
			}

			comboboxTargets.Model = store;
		}

		void OnRegisterClicked (object sender, EventArgs e)
		{
			(registerdialog = new RegisterDialog {
				TransientFor = this,
			}).ShowAll ();

			var returnvalue = (ResponseType)registerdialog.Run ();
			registerdialog.Hide ();

			if (returnvalue == ResponseType.Ok) {
				targets.Add (new Target (registerdialog.TargetName, registerdialog.Url, registerdialog.Email,registerdialog.Password));
				FillCombo ();
				comboboxTargets.Active = targets.Count - 1;
			}
		}


		void OnTargetChanged (object sender, EventArgs e)
		{
			buttonPublish.Sensitive = false;
			nameEntry.Sensitive = authorEntry.Sensitive = versionEntry.Sensitive = lonDescriptionEntry.Sensitive = 
				descriptionEntry.Sensitive = versionNotesEntry.Sensitive = false;
			var proj = PublishHandler.GetActiveMobileProject();
			var conf = proj.GetConfiguration(IdeApp.Workspace.ActiveConfiguration) as DotNetProjectConfiguration;
			publisher = new EasePublisher (proj, conf,  TargetUrl, TargetName, TargetEmail, TargetPassword, null);
			publisher.GetList (OnAuthenticated, OnSuccess, null);
		}

		void OnAuthenticated (string targetname)
		{
			SaveTargets ();
		}

		void OnSuccess ()
		{
			buttonPublish.Sensitive = true;
			nameEntry.Sensitive = authorEntry.Sensitive = versionEntry.Sensitive = lonDescriptionEntry.Sensitive = 
				descriptionEntry.Sensitive = versionNotesEntry.Sensitive = true;
				
			if (publisher.Metadata != null) {
				descriptionEntry.Text = publisher.Metadata.ShortDescription;
				lonDescriptionEntry.Buffer.Text = publisher.Metadata.LongDescription;
				versionNotesEntry.Buffer.Text = publisher.Metadata.VersionNotes;
			}
		}

		public class TargetsSettings {
			public List<Target> Targets { get; set; }
		}

		public class Target {

			public Target (string name, string url, string email, string password)
			{
				Name = name;
				Url = url;
				Email = email;
				Password = password;
			}

			public string Name { get; set; }
			public string Url { get; set; }
			public string Email { get; set; }
			public string Password { get; set; }
		}
	}
}
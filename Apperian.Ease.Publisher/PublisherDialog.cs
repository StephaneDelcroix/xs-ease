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

using Gtk;

namespace Apperian.Ease.Publisher
{
	public partial class PublisherDialog : Dialog
	{
		RegisterDialog registerdialog;
		List<Tuple<string,string,string,string>> targets;
		EasePublisher publisher;
		public PublisherDialog ()
		{
			this.Build ();

			LoadTargets ();
			FillCombo ();

			nameEntry.Sensitive = authorEntry.Sensitive = versionEntry.Sensitive = descriptionEntry.Sensitive = versionNotesEntry.Sensitive = false;

			buttonRegister.Clicked += OnRegisterClicked;
			comboboxTargets.Changed += OnTargetChanged;
			buttonPublish.Sensitive = false;
		}

		public string TargetName { get { return targets[comboboxTargets.Active].Item1; }}
		public string TargetUrl { get { return targets[comboboxTargets.Active].Item2; }}
		public string TargetEmail { get { return targets[comboboxTargets.Active].Item3; }}
		public string TargetPassword { get { return targets[comboboxTargets.Active].Item4; }}
		public EaseMetadata Metadata { get { return new EaseMetadata {
					Author = authorEntry.Text,
					LongDescription = descriptionEntry.Text,
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
			targets = new List<Tuple<string,string,string,string>> ();
			var prefs = EasePublisher.Project.UserProperties.GetValue<string> (settingsKey);
			if (prefs == null)
				return;
			var splitted = prefs.Split (new [] {'|'});
			if (splitted.Length % 4 == 0) {
				for (var i=0;i<splitted.Length/4;i+=4)
					targets.Add (new Tuple<string, string, string, string> (splitted[i], splitted[i+1], splitted[i+2], splitted[i+3]));
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
				Console.WriteLine (item.Item1);
				store.AppendValues (item.Item1);
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
				targets.Add (new Tuple<string, string, string, string> (registerdialog.TargetName, registerdialog.Url, registerdialog.Email,registerdialog.Password));
				FillCombo ();
				comboboxTargets.Active = targets.Count - 1;
			}
		}


		void OnTargetChanged (object sender, EventArgs e)
		{
			buttonPublish.Sensitive = false;
			nameEntry.Sensitive = authorEntry.Sensitive = versionEntry.Sensitive = descriptionEntry.Sensitive = versionNotesEntry.Sensitive = false;
			publisher = new EasePublisher (TargetUrl, TargetName, TargetEmail, TargetPassword, null);
			publisher.GetList (OnAuthenticated, OnSuccess, null);
		}

		void OnAuthenticated (string targetname)
		{
			var prefs = "";
			foreach (var t in targets)
				prefs += String.Format ("{0}|{1}|{2}|{3}|", t.Item1, t.Item2, t.Item3, t.Item4);
			if (prefs.Length > 0)
				prefs = prefs.Substring (0, prefs.Length-1);
			EasePublisher.Project.UserProperties.SetValue<string> (settingsKey, prefs);
		}

		void OnSuccess ()
		{
			buttonPublish.Sensitive = true;
			nameEntry.Sensitive = authorEntry.Sensitive = versionEntry.Sensitive = descriptionEntry.Sensitive = versionNotesEntry.Sensitive = true;
			nameEntry.Text = publisher.ApplicationName;
			authorEntry.Text = EasePublisher.Project.AuthorInformation.Name;
			versionEntry.Text = EasePublisher.Project.Version;
				
			if (publisher.Metadata != null) {
				descriptionEntry.Text = publisher.Metadata.ShortDescription;
				versionNotesEntry.Buffer.Text = publisher.Metadata.VersionNotes;
			}
		
		}
	}
}
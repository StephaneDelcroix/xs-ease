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

		public PublisherDialog ()
		{
			this.Build ();

			LoadTargets ();
			FillCombo ();

			buttonRegister.Clicked += OnRegisterClicked;
			comboboxTargets.Changed += OnTargetChanged;
			buttonPublish.Sensitive = targets.Count > 0 && comboboxTargets.Active >=0;
		}

		public string TargetName { get { return targets[comboboxTargets.Active].Item1; }}
		public string TargetUrl { get { return targets[comboboxTargets.Active].Item2; }}
		public string TargetEmail { get { return targets[comboboxTargets.Active].Item3; }}
		public string TargetPassword { get { return targets[comboboxTargets.Active].Item4; }}

		void LoadTargets ()
		{
			targets = new List<Tuple<string,string,string,string>> ();
		}

		void FillCombo ()
		{
			comboboxTargets.Clear ();
			var cell = new CellRendererText ();
			comboboxTargets.PackStart (cell, false);
			comboboxTargets.AddAttribute (cell, "text", 0);
			var store = new ListStore(typeof (string));
			comboboxTargets.Model = store;

			foreach (var item in targets)
				store.AppendValues (item.Item1);
		}

		void OnRegisterClicked (object sender, EventArgs e)
		{
			(registerdialog = new RegisterDialog {
				TransientFor = this,
			}).ShowAll ();

			var returnvalue = (ResponseType)registerdialog.Run ();
			registerdialog.Hide ();

			if (returnvalue == ResponseType.Ok) {
				targets.Add (new Tuple<string, string, string, string> (registerdialog.Name, registerdialog.Url, registerdialog.Email,registerdialog.Password));
				FillCombo ();
			}
		}

		void OnTargetChanged (object sender, EventArgs e)
		{
			buttonPublish.Sensitive = targets.Count > 0 && comboboxTargets.Active >=0;
		}
	}
}
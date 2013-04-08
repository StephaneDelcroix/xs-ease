//
// PublisherDialog.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;

using Gtk;

namespace Apperian.Ease.Publisher
{
	public partial class PublisherDialog : Dialog
	{
		Dialog registerdialog;

		public PublisherDialog ()
		{
			this.Build ();

			buttonRegister.Clicked += (object sender, EventArgs e) => {
				registerdialog = new RegisterDialog ();
				registerdialog.TransientFor = this;
				registerdialog.ShowAll ();
				var returnvalue = registerdialog.Run ();
				registerdialog.Hide ();
				Console.WriteLine ((ResponseType)returnvalue);
			};
		}
	}
}


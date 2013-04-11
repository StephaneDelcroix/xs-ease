//
// RegisterDialog.cs
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
	public partial class RegisterDialog : Dialog
	{
		public RegisterDialog ()
		{
			this.Build ();
		}

		public string Url { get { return entryURL.Text; }}
		public string TargetName { get { return entryName.Text; }}
		public string Email { get { return entryEmail.Text; }}
		public string Password { get { return entryPassword.Text; }} 
	}
}


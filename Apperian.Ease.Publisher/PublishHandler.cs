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

namespace Apperian.Ease.Publisher
{
	public class PublishHandler : CommandHandler
	{
		protected override void Run ()
		{
#if DEBUG
			Console.WriteLine ("Run");
#endif
			var dialog = new PublisherDialog ();
			dialog.ShowAll ();
			dialog.Run ();
			dialog.Hide ();
		}

		protected override void Update (CommandInfo info)
		{
			Console.WriteLine ("Update");
		}
	}
}


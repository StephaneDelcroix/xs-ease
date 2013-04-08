//
// PublishHandler.cs
//
// Author:
//       Stephane Delcroix <stephane@delcroix.org>
//
// Copyright (c) 2013 S. Delcroix
//
using System;
using MonoDevelop.Components.Commands;

namespace Apperian.Ease.Publisher
{
	public class PublishHandler : CommandHandler
	{
		protected override void Run ()
		{
			Console.WriteLine ("Run");
		}

		protected override void Update (CommandInfo info)
		{
			Console.WriteLine ("Update");
		}
	}
}


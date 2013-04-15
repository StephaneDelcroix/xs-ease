//
// AuthenticateResult.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;

namespace Apperian.Ease.Publisher
{
	public class AuthenticateResult : Result
	{
		public AResult Result { get; set; }

		public class AResult {
			public string Token { get ;set; }
		}
	}
}
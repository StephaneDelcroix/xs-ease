//
// AuthenticateRequest.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;

namespace Apperian.Ease.Publisher
{
	public class AuthenticateRequest : JsonRequest<AuthenticateResult>
	{
		public AuthenticateRequest (string url, string email, string password, Action<AuthenticateResult> success, Action<Exception>error) : base (success, error)
		{
			Uri = url;
			Method = "POST";
			RequestContent = String.Format ("{{\"id\": 1, \"apiVersion\": \"1.0\", \"method\": \"com.apperian.eas.user.authenticateuser\", \"params\": {{\"email\": \"{0}\", \"password\": \"{1}\"}}, \"jsonrpc\": \"2.0\"}}", email, password );
		}
	}
}
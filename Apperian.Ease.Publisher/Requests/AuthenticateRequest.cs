//
// AuthenticateRequest.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;
using System.Json;

namespace Apperian.Ease.Publisher
{
	public class AuthenticateRequest : JsonRequest
	{
		public AuthenticateRequest (Action<string> success, Action<Exception> error) : base ((new SuccessHandler (success, error)).GetContent, error)
		{
		}

		public void Authenticate (string url, string email, string password)
		{
			var content = String.Format ("{{\"id\": 1, \"apiVersion\": \"1.0\", \"method\": \"com.apperian.eas.user.authenticateuser\", \"params\": {{\"email\": \"{0}\", \"password\": \"{1}\"}}, \"jsonrpc\": \"2.0\"}}", email, password );
#if DEBUG
			Console.WriteLine (content);
#endif
			Send (url, "POST", content);
		}

		class SuccessHandler {
			Action<string> getToken;
			Action<Exception> error;

			public SuccessHandler (Action<string> getToken, Action<Exception> error)
			{
				this.getToken = getToken;
				this.error = error;
			}

			public void GetContent (string content)
			{
#if DEBUG
				Console.WriteLine (content);
#endif
				JsonValue result = JsonValue.Parse (content);
				if (result.ContainsKey ("error")) {
					string errormessage = String.Format ("{0} - {1}", result["error"]["message"], result["error"]["data"]["detailedMessage"]);
#if DEBUG 
					Console.WriteLine (">>>ERROR>>>" + errormessage);
#endif
					error (new Exception (errormessage));
				}
				else if (result.ContainsKey ("result"))
					getToken (result["result"]["token"]);
			}
		}
	}
}
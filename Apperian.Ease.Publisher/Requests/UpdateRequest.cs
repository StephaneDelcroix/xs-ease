//
// UpdateRequest.cs
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
	public class UpdateRequest: JsonRequest
	{
		public UpdateRequest (Action<Transaction> success, Action<Exception> error) : base ((new SuccessHandler (success, error)).GetContent, error)
		{
		}
		
		public void Update (string url, string appId, string token)
		{
			var content = String.Format ("{{\"id\": 1, \"apiVersion\": \"1.0\", \"method\": \"com.apperian.eas.apps.update\", \"params\": {{\"appID\": \"{0}\", \"token\": \"{1}\"}}, \"jsonrpc\": \"2.0\"}}", appId, token);
#if DEBUG
			Console.WriteLine (content);
#endif
			Send (url, "POST", content);
		}
		
		class SuccessHandler {
			Action<Transaction> getTransaction;
			Action<Exception> error;
			
			public SuccessHandler (Action<Transaction> getTransaction, Action<Exception> error)
			{
				this.getTransaction = getTransaction;
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
					getTransaction (new Transaction {FileUploadUrl = result["result"]["fileUploadURL"], Id = result["result"]["transactionID"]});
			}
		}
	}
}


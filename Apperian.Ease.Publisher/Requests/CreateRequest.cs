//
// CreateRequest.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;

namespace Apperian.Ease.Publisher
{
	public class CreateRequest : JsonRequest<CreateOrUpdateResult>
	{
		public CreateRequest (string url, string token, Action<CreateOrUpdateResult> success, Action<Exception> error) : base (success, error)
		{
			RequestContent = String.Format ("{{\"id\": 1, \"apiVersion\": \"1.0\", \"method\": \"com.apperian.eas.apps.create\", \"params\": {{\"token\": \"{0}\"}}, \"jsonrpc\": \"2.0\"}}", token);
			Uri = url;
			Method = "POST";
		}
		/*
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
		}*/
	}
}


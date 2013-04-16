//
// UpdateRequest.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;

namespace Apperian.Ease.Publisher
{
	public class UpdateRequest: JsonRequest<CreateOrUpdateResult>
	{
		public UpdateRequest (string url, string appId, string token, Action<CreateOrUpdateResult> success, Action<Exception> error) : base (success, error)
		{
			RequestContent = String.Format ("{{\"id\": 1, \"apiVersion\": \"1.0\", \"method\": \"com.apperian.eas.apps.update\", \"params\": {{\"appID\": \"{0}\", \"token\": \"{1}\"}}, \"jsonrpc\": \"2.0\"}}", appId, token);
			Uri = url;
			Method = "POST";
		}
	}
}
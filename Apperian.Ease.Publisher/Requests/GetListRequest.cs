//
// GetListRequest.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;

namespace Apperian.Ease.Publisher
{
	public class GetListRequest : JsonRequest<GetListResult>
	{
		public GetListRequest (string url, string token, Action<GetListResult> success, Action<Exception> error) : base (success, error)
		{
			RequestContent = String.Format ("{{\"id\": 1, \"apiVersion\": \"1.0\", \"method\": \"com.apperian.eas.apps.getlist\", \"params\": {{\"token\": \"{0}\"}}, \"jsonrpc\": \"2.0\"}}", token);
			Uri = url;
			Method = "POST";
		}
	}
}
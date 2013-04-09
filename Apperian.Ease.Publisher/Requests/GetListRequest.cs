//
// GetListRequest.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;
using System.Json;
using System.Collections.Generic;

namespace Apperian.Ease.Publisher
{
	public class GetListRequest : JsonRequest
	{
		public GetListRequest (Action<IList<EaseApplication>> success, Action<Exception> error) : base ((new SuccessHandler (success, error)).GetContent, error)
		{
		}

		public void GetList (string url, string token)
		{
			var content = String.Format ("{{\"id\": 1, \"apiVersion\": \"1.0\", \"method\": \"com.apperian.eas.apps.getlist\", \"params\": {{\"token\": \"{0}\"}}, \"jsonrpc\": \"2.0\"}}", token);
#if DEBUG
			Console.WriteLine (content);
#endif
			Send (url, "POST", content);
		}

		class SuccessHandler {
			Action<IList<EaseApplication>> getAppList;
			Action<Exception> error;

			public SuccessHandler (Action<IList<EaseApplication>> getAppList, Action<Exception> error)
			{
				this.getAppList = getAppList;
				this.error = error;
			}

			public void GetContent (string content)
			{
#if DEBUG
				Console.WriteLine (content);
#endif
			}
		}
	}
}

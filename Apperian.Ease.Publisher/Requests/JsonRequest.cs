//
// JsonRequest.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;
using System.IO;
using System.Net;

namespace Apperian.Ease.Publisher
{
	public abstract class JsonRequest
	{
		protected readonly Action<string> Success;
		protected readonly Action<Exception> Error;

		public JsonRequest (Action<string> success, Action<Exception> error)
		{
			this.Success = success;
			this.Error = error;
		}

		static HttpWebRequest Create (string method, Uri uri)
		{
			var request = (HttpWebRequest) WebRequest.Create (uri);
			request.ContentType = "application/json; charset=utf-8";
			request.Accept = "application/json";
			request.Method = method;
			
			return request;
		}

		protected void Send (string url, string method, string content)
		{
			try {
				var request = Create (method, new Uri (url));
				using (var writer = new StreamWriter (request.GetRequestStream ())) {
					writer.Write (content);
				}
				request.BeginGetResponse (ReadCallback, request);
			} catch (Exception e) {
				Error (e);
			}
		}

		void ReadCallback (IAsyncResult asyncResult) 
		{
			try {
				var request = (HttpWebRequest)asyncResult.AsyncState;
				var response = (HttpWebResponse)request.EndGetResponse (asyncResult);
				using (var reader = new StreamReader (response.GetResponseStream ())) {
					Success (reader.ReadToEnd ());
				}
			} catch (Exception e) {
				Error (e);
			}
		}
	}
}
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

using Newtonsoft.Json;

namespace Apperian.Ease.Publisher
{
	public abstract class JsonRequest<T> where T : Result
	{
		protected readonly Action<T> Success;
		protected readonly Action<Exception> Error;
		protected string Uri { get; set;}
		protected string Method { get;set;}
		protected string RequestContent { get; set; }

		public JsonRequest (Action<T> success, Action<Exception> error)
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

		public virtual void Send ()
		{
#if DEBUG
			Console.WriteLine (RequestContent);
#endif
			try {
				var request = Create (Method, new Uri (Uri));
				using (var writer = new StreamWriter (request.GetRequestStream ())) {
					writer.Write (RequestContent);
				}
				request.BeginGetResponse (ReadCallback, request);
			} catch (Exception e) {
				Error (e);
			}
		}

		protected void ReadCallback (IAsyncResult asyncResult) 
		{
			try {
				var request = (HttpWebRequest)asyncResult.AsyncState;
				var response = (HttpWebResponse)request.EndGetResponse (asyncResult);
				using (var reader = new StreamReader (response.GetResponseStream ())) {
					HandleResponse (reader);
				}
			} catch (Exception e) {
				Error (e);
			}
		}

		protected virtual void HandleResponse (StreamReader streamReader)
		{
			try {
				using (var jsonReader = new JsonTextReader (streamReader)) {
					var serializer = new JsonSerializer ();
					var @object = serializer.Deserialize<T> (jsonReader);
					if (@object.Error == null)
						Success(@object);
					else
						Error (new Exception (String.Format ("{0} - {1}", @object.Error.Message, @object.Error.Data == null ? null : @object.Error.Data.DetailedMessage)));
				}
			} catch (Exception e) {
				Error (e);
			}
		}
	}
}
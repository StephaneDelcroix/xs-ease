//
// PublishRequest.cs
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
	public class PublishRequest : JsonRequest
	{
		public PublishRequest (Action<string> success, Action<Exception> error) : base ((new SuccessHandler (success, error)).GetContent, error)
		{
		}

		public void Publish (string url, string token, string transactionid, string fileId, EaseMetadata metadata)
		{
			var content = String.Format ("{{\"id\": 1, \"apiVersion\": \"1.0\", \"method\": \"com.apperian.eas.apps.publish\", \"params\": {{\"EASEmetadata\" :  {{ \"author\" : \"{0}\", \"longdescription\" : \"{1}\", \"name\" : \"{2}\", \"shortdescription\" : \"{3}\", \"version\" : \"{4}\", \"versionNotes\" : \"{5}\"}}, \"files\" :  {{ \"application\" : \"{6}\"}}, \"token\" : \"{7}\", \"transactionID\" : \"{8}\"}}, \"jsonrpc\": \"2.0\"}}",
			                             metadata.Author, metadata.LongDescription, metadata.Name, metadata.ShortDescription, metadata.Version, metadata.VersionNotes,
			                             fileId, token, transactionid);
#if DEBUG
			Console.WriteLine (content);
#endif
			Send (url, "POST", content);
		}
		class SuccessHandler {
			Action<string> getAppid;
			Action<Exception> error;
			
			public SuccessHandler (Action<string> getAppid, Action<Exception> error)
			{
				this.getAppid = getAppid;
				this.error = error;
			}
			
			public void GetContent (string content)
			{
#if DEBUG
				Console.WriteLine (content);
#endif
				JsonValue result = JsonValue.Parse (content);
				if (result.ContainsKey ("error")) {
					string errormessage = String.Format ("{0}", result["error"]["message"]);
#if DEBUG 
					Console.WriteLine (">>>ERROR>>>" + errormessage);
#endif
					error (new Exception (errormessage));
				}
				else if (result.ContainsKey ("result"))
					getAppid (result["result"]["appID"]);
			}
		}
	}
}


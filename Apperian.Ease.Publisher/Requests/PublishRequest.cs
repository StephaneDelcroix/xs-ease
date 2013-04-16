//
// PublishRequest.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;

namespace Apperian.Ease.Publisher
{
	public class PublishRequest : JsonRequest<PublishResult>
	{
		public PublishRequest (string url, string token, string transactionid, string fileId, EaseMetadata metadata, Action<PublishResult> success, Action<Exception> error) : base (success, error)
		{
			RequestContent = String.Format ("{{\"id\": 1, \"apiVersion\": \"1.0\", \"method\": \"com.apperian.eas.apps.publish\", \"params\": {{\"EASEmetadata\" :  {{ \"author\" : \"{0}\", \"longdescription\" : \"{1}\", \"name\" : \"{2}\", \"shortdescription\" : \"{3}\", \"version\" : \"{4}\", \"versionNotes\" : \"{5}\"}}, \"files\" :  {{ \"application\" : \"{6}\"}}, \"token\" : \"{7}\", \"transactionID\" : \"{8}\"}}, \"jsonrpc\": \"2.0\"}}",
			                             metadata.Author, metadata.LongDescription, metadata.Name, metadata.ShortDescription, metadata.Version, metadata.VersionNotes,
			                             fileId, token, transactionid);
			Method = "POST";
			Uri = url;
		}
	}
}
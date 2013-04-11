//
// UploadRequest.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Web;
using System.Text;
using System.Net;
using System.Json;

namespace Apperian.Ease.Publisher
{
	public class UploadRequest
	{
		Action<string> success;
		Action<Exception> error;

		public UploadRequest (Action<string> success, Action<Exception> error)
		{
			this.success = success;
			this.error = error;
		}

		public void Upload (string fileUploadUrl, string transactionId, string ipa)
		{
#if DEBUG
			Console.WriteLine (fileUploadUrl);
			Console.WriteLine (ipa);
#endif
			try {
				var content = UploadFileEx (ipa, fileUploadUrl, "LUuploadFile");
#if DEBUG
				Console.WriteLine (content);
#endif
				JsonValue result = JsonValue.Parse (content);
				if (result.ContainsKey ("fileID")) {
					success (result["fileID"]);
				}
				else {
					string errormessage = String.Format ("{0} - {1}", result["error"]["message"], result["error"]["data"]["detailedMessage"]);
#if DEBUG 
					Console.WriteLine (">>>ERROR>>>" + errormessage);
#endif
					error (new Exception (errormessage));
				}
			} catch (Exception e)
			{
				error (e);
			}
		}

		public static string UploadFileEx( string uploadfile, string url, 
		                                  string fileFormName = null, string contenttype = null)
		{
			if (string.IsNullOrEmpty (fileFormName))
				fileFormName = "file";

			if (string.IsNullOrEmpty (contenttype))
				contenttype = "application/octet-stream";

			Uri uri = new Uri(url);
			
			string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
			HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(uri);
			webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
			//webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
			webrequest.Method = "POST";
			
			// Build up the post message header
			StringBuilder sb = new StringBuilder();
			sb.Append("--");
			sb.Append(boundary);
			sb.Append("\r\n");
			sb.Append("Content-Disposition: form-data; name=\"");
			//sb.Append("Content-Disposition: file; name=\"");
			sb.Append(fileFormName);
			sb.Append("\"; filename=\"");
			sb.Append(Path.GetFileName(uploadfile));
			sb.Append("\"");
			sb.Append("\r\n");
			sb.Append("Content-Type: ");
			sb.Append(contenttype);
			sb.Append("\r\n");
			sb.Append("\r\n");            
			
			string postHeader = sb.ToString();
			byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);
			
			// Build the trailing boundary string as a byte array
			// ensuring the boundary appears on a line by itself
			byte[] boundaryBytes = 
				Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
			
			FileStream fileStream = new FileStream(uploadfile, 
			                                       FileMode.Open, FileAccess.Read);
			long length = postHeaderBytes.Length + fileStream.Length + 
				boundaryBytes.Length;
			webrequest.ContentLength = length;
			
			Stream requestStream = webrequest.GetRequestStream();
			
			// Write out our post header
			requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
			
			// Write out the file contents
			byte[] buffer = new Byte[checked((uint)Math.Min(4096, 
			                                                (int)fileStream.Length))];
			int bytesRead = 0;
			while ( (bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0 )
				requestStream.Write(buffer, 0, bytesRead);
			
			// Write out the trailing boundary
			requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);

			WebResponse response = webrequest.GetResponse();
			Stream s = response.GetResponseStream();
			StreamReader sr = new StreamReader(s);
			
			return sr.ReadToEnd();
		}
	}
}


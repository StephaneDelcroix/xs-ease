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
	public class GetListRequest : JsonRequest<GetListResult>
	{
		public GetListRequest (string url, string token, Action<GetListResult> success, Action<Exception> error) : base (success, error)
		{
			RequestContent = String.Format ("{{\"id\": 1, \"apiVersion\": \"1.0\", \"method\": \"com.apperian.eas.apps.getlist\", \"params\": {{\"token\": \"{0}\"}}, \"jsonrpc\": \"2.0\"}}", token);
			Uri = url;
			Method = "POST";
		}

		/*
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
				JsonValue result = JsonValue.Parse (content);
				if (result.ContainsKey ("error")) {
					string errormessage = String.Format ("{0} - {1}", result["error"]["message"], result["error"]["data"]["detailedMessage"]);
#if DEBUG 
					Console.WriteLine (">>>ERROR>>>" + errormessage);
#endif
					error (new Exception (errormessage));
				}
				else if (result.ContainsKey ("result")) {
					var list = new List<EaseApplication> ();
					foreach (var app in result["result"]["applications"] as JsonArray) {
#if DEBUG
						Console.WriteLine ("{0} - {1}", app["name"], app["type"]);
#endif
						var easeApp = new EaseApplication {
							ApplicationType = app ["type"],
							Name = app ["name"],
							Id = app ["ID"],
							Metadata = new EaseMetadata {
								Author = app ["author"],
								Name = app ["name"],
								ShortDescription = app ["shortdescription"],
								LongDescription = app ["longdescription"],
								Version = app ["version"],
								VersionNotes = app ["versionNotes"],
							},
						};
						list.Add (easeApp);
					}
					getAppList (list);
				}
			}

		}*/
	}
}
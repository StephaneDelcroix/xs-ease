//
// GetListResult.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Apperian.Ease.Publisher
{
	public class GetListResult : Result
	{
		public GResult Result { get; set; }

		public class GResult {
			public List<Application> Applications { get; set; }
		}

		public class Application {
			public string ID { get; set; }
			public string Author { get; set; }
			public string BundleID { get; set; }
			public string LongDescription { get; set; }
			public string Name { get; set; }
			public string ShortDescription { get; set; }
			public string Status { get; set; }
			[JsonProperty ("type")]
			public string ApplicationType { get; set; }
			public string Version { get; set; }
			public string VersionNotes { get; set; }
		}
	}
}


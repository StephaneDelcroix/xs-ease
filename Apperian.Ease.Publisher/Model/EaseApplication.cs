//
// EaseApplication.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//

namespace Apperian.Ease.Publisher
{
	public class EaseApplication
	{
		public string ApplicationType { get; set; }
		public string Name { get; set; }
		public string Id { get; set; }
		public EaseMetadata Metadata { get; set; }
	}
}
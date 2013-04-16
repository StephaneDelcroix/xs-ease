//
// PublishResult.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//

namespace Apperian.Ease.Publisher
{
	public class PublishResult : Result
	{
		public PResult Result { get; set; }

		public class PResult {
			public string AppId { get; set; }
			public string Status { get; set; }
		}
	}
}
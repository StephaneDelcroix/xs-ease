//
// CreateOrUpdateResult.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//

namespace Apperian.Ease.Publisher
{
	public class CreateOrUpdateResult : Result
	{
		public CResult Result { get; set; }

		public class CResult {
			public string FileUploadURL { get; set; }
			public string TransactionID { get; set; }
		}
	}
}


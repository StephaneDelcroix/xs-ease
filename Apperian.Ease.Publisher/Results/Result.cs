//
// Result.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;

namespace Apperian.Ease.Publisher
{
	public abstract class Result
	{
		public string Id { get; set; }
		public string JsonRpc { get; set; }

		public CError Error { get; set; }

		public class Data
		{
			public string DetailedMessage { get; set; }
			public string Guid { get; set; }
		}
		
		public class CError
		{
			public int Code { get; set; }
			public string Message { get; set; }
			public Data Data { get; set; }
		}
	}
}


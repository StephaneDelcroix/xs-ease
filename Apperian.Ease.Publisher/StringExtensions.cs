//
// StringExtensions.cs
//
// Author:
//       Stephane Delcroix <stephane@mi8.be>
//
// Copyright (c) 2013 Apperian, Inc.
//
using System;

namespace Apperian.Ease.Publisher
{
	public static class StringExtensions
	{
		public static string TrimAt (this string s, int length)
		{
			if (s.Length <= length)
				return s;
			return s.Substring (0, length);
		}
	}
}
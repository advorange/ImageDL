﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Instagram.Models
{
	/// <summary>
	/// Not sure what this holds.
	/// </summary>
	public struct InstagramRelatedMediaInfo
	{
		/// <summary>
		/// Not sure.
		/// </summary>
		[JsonProperty("edges")]
		public List<object> Nodes { get; private set; }
	}
}
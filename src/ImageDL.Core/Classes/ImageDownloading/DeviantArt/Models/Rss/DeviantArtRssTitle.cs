﻿using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.DeviantArt.Models.Rss
{
	/// <summary>
	/// A title for an image.
	/// </summary>
	public struct DeviantArtRssTitle
	{
		/// <summary>
		/// The text of the title.
		/// </summary>
		[JsonProperty("text")]
		public string Text { get; private set; }

		/// <summary>
		/// The type of title. Is either html or plain.
		/// </summary>
		[JsonProperty("type")]
		public string Type { get; private set; }
	}
}
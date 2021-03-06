﻿using System;

using Newtonsoft.Json;

namespace ImageDL.Classes.ImageDownloading.Booru.Konachan.Models
{
	/// <summary>
	/// Json model for a frame, whatever that is.
	/// </summary>
	public struct KonachanFrame
	{
		/// <summary>
		/// The height of the image.
		/// </summary>
		[JsonProperty("height")]
		public int Height { get; private set; }

		/// <summary>
		/// The id of the post this belongs to.
		/// </summary>
		[JsonProperty("post_id")]
		public int PostId { get; private set; }

		/// <summary>
		/// The preview height.
		/// </summary>
		[JsonProperty("preview_height")]
		public int PreviewHeight { get; private set; }

		/// <summary>
		/// The url to the preview.
		/// </summary>
		[JsonProperty("preview_url")]
		public Uri PreviewUrl { get; private set; }

		/// <summary>
		/// The preview width.
		/// </summary>
		[JsonProperty("preview_width")]
		public int PreviewWidth { get; private set; }

		/// <summary>
		/// The height of the frame.
		/// </summary>
		[JsonProperty("source_height")]
		public int SourceHeight { get; private set; }

		/// <summary>
		/// The left most part of the frame.
		/// </summary>
		[JsonProperty("source_left")]
		public int SourceLeft { get; private set; }

		/// <summary>
		/// The top most part of the frame.
		/// </summary>
		[JsonProperty("source_top")]
		public int SourceTop { get; private set; }

		/// <summary>
		/// The width of the frame.
		/// </summary>
		[JsonProperty("source_width")]
		public int SourceWidth { get; private set; }

		/// <summary>
		/// The url to the frame.
		/// </summary>
		[JsonProperty("url")]
		public Uri Url { get; private set; }

		/// <summary>
		/// The width of the image.
		/// </summary>
		[JsonProperty("width")]
		public int Width { get; private set; }
	}
}
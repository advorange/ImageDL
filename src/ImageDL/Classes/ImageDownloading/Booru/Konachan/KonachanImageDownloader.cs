﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;

namespace ImageDL.Classes.ImageDownloading.Booru.Konachan
{
	/// <summary>
	/// Downloads images from Konachan.
	/// </summary>
	public sealed class KonachanImageDownloader : BooruImageDownloader<KonachanPost>
	{
		/// <summary>
		/// Creates an instance of <see cref="KonachanImageDownloader"/>.
		/// </summary>
		public KonachanImageDownloader() : base("Konachan", 6) { }

		/// <inheritdoc />
		protected override string GenerateQuery(int page)
		{
			return $"https://www.konachan.com/post.json" +
				$"?limit=100" +
				$"&tags={WebUtility.UrlEncode(Tags)}" +
				$"&page={page}";
		}
		/// <inheritdoc />
		protected override List<KonachanPost> Parse(string text)
		{
			return JsonConvert.DeserializeObject<List<KonachanPost>>(text);
		}
	}
}
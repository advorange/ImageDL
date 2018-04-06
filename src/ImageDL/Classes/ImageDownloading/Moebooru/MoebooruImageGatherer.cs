﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading.Moebooru.Models;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Moebooru
{
	/// <summary>
	/// Gathers images from a specified Moebooru based website link.
	/// </summary>
	public abstract class MoebooruImageGatherer : IImageGatherer
	{
		private readonly string _Name;
		private readonly string _Search;
		private readonly Func<ImageDownloaderClient, string, Task<MoebooruPost>> _Func;

		/// <summary>
		/// Creates an instance of <see cref="MoebooruImageGatherer"/>.
		/// </summary>
		/// <param name="name">The name of the website..</param>
		/// <param name="search">The term to search for directly before an id.</param>
		/// <param name="func">The function to get posts with.</param>
		public MoebooruImageGatherer(string name, string search, Func<ImageDownloaderClient, string, Task<MoebooruPost>> func)
		{
			_Name = name;
			_Search = search;
			_Func = func;
		}

		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains(_Name);
		}
		/// <inheritdoc />
		public async Task<GatheredImagesResponse> GetImagesAsync(ImageDownloaderClient client, Uri url)
		{
			var u = ImageDownloaderClient.RemoveQuery(url).ToString();
			if (u.CaseInsIndexOf(_Search, out var index))
			{
				var id = u.Substring(index + _Search.Length).Split('/').First();
				var post = await _Func(client, id).CAF();
				return post == null
					? GatheredImagesResponse.FromNotFound(url)
					: GatheredImagesResponse.FromGatherer(url, post.ContentUrls.ToArray());
			}
			return GatheredImagesResponse.FromUnknown(url);
		}
	}
}
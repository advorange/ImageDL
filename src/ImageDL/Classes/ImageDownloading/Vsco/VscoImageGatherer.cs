﻿using System;
using System.Linq;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Vsco
{
	/// <summary>
	/// Gathers images from a specified Vsco link.
	/// </summary>
	public sealed class VscoImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("vsco.co");
		}
		/// <inheritdoc />
		public async Task<GatheredImagesResponse> GetImagesAsync(ImageDownloaderClient client, Uri url)
		{
			var u = ImageDownloaderClient.RemoveQuery(url).ToString();
			var search = "/media/";
			if (u.CaseInsIndexOf(search, out var index))
			{
				var id = u.Substring(index + search.Length).Split('/').First();
				var post = await VscoImageDownloader.GetVscoPostAsync(client, id).CAF();
				return post == null
					? GatheredImagesResponse.FromNotFound(url)
					: GatheredImagesResponse.FromGatherer(url, post.ContentUrls.ToArray());
			}
			return GatheredImagesResponse.FromUnknown(url);
		}
	}
}
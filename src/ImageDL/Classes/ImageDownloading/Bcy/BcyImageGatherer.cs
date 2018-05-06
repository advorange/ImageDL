﻿using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Bcy
{
	/// <summary>
	/// Gathers images from a specified Bcy link.
	/// </summary>
	public struct BcyImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("bcy.net");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			return await BcyImageDownloader.GetBcyImagesAsync(client, url).CAF();
		}
	}
}
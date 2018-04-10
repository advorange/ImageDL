﻿using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Classes.ImageDownloading.Eshuushuu.Models;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.Eshuushuu
{
	/// <summary>
	/// Gathers images from a specified Eshuushuu link.
	/// </summary>
	public sealed class EshuushuuImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("e-shuushuu.net");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			return await EshuushuuImageDownloader.GetEshuushuuImagesAsync(client, url).CAF();
		}
	}
}
﻿using System;
using System.Threading.Tasks;
using AdvorangesUtils;
using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.AnimePictures
{
	/// <summary>
	/// Gathers images from a specified AnimePictures link.
	/// </summary>
	public sealed class AnimePicturesImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
		{
			return url.Host.CaseInsContains("anime-pictures.net");
		}
		/// <inheritdoc />
		public async Task<ImageResponse> FindImagesAsync(IImageDownloaderClient client, Uri url)
		{
			return await AnimePicturesImageDownloader.GetAnimePicturesImagesAsync(client, url).CAF();
		}
	}
}
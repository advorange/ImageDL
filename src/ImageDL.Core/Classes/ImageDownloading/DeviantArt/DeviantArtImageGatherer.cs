﻿using System;
using System.Threading.Tasks;

using AdvorangesUtils;

using ImageDL.Interfaces;

namespace ImageDL.Classes.ImageDownloading.DeviantArt
{
	/// <summary>
	/// Gathers images from a specified DeviantArt link.
	/// </summary>
	public struct DeviantArtImageGatherer : IImageGatherer
	{
		/// <inheritdoc />
		public Task<ImageResponse> FindImagesAsync(IDownloaderClient client, Uri url)
			=> DeviantArtPostDownloader.GetDeviantArtImagesAsync(client, url);

		/// <inheritdoc />
		public bool IsFromWebsite(Uri url)
			=> url.Host.CaseInsContains("deviantart.com");
	}
}
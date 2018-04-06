﻿using System;
using System.Threading.Tasks;
using ImageDL.Classes;
using ImageDL.Classes.ImageDownloading;

namespace ImageDL.Interfaces
{
	/// <summary>
	/// Interface for something that can grab the images of a post.
	/// </summary>
	public interface IImageGatherer
	{
		/// <summary>
		/// Returns true if the uri is from the website.
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		bool IsFromWebsite(Uri url);
		/// <summary>
		/// Attempts to get images from a uri.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		Task<GatheredImagesResponse> GetImagesAsync(ImageDownloaderClient client, Uri url);
	}
}
﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ImageDL.Classes;
using ImageDL.Classes.SettingParsing;

namespace ImageDL.Interfaces
{
	/// <summary>
	/// Interface for something that can download images.
	/// </summary>
	public interface IImageDownloader
	{
		/// <summary>
		/// Used to set arguments via command line.
		/// </summary>
		SettingParser SettingParser { get; }
		/// <summary>
		/// Indicates that all arguments have been set and that the user wants the downloader to start.
		/// </summary>
		bool CanStart { get; }

		/// <summary>
		/// Downloads all the images that match the supplied arguments then saves all the found animated content links.
		/// </summary>
		/// <param name="services">Holds the services. Should at least hold a downloader client.</param>
		/// <param name="token">Cancellation token for a semaphore slim that makes sure only one instance of downloading is happening.</param>
		/// <returns>An awaitable task which downloads images.</returns>
		Task<DownloaderResponse> StartAsync(IServiceProvider services, CancellationToken token = default);
		/// <summary>
		/// Gathers the posts which match the supplied settings.
		/// </summary>
		/// <param name="client">The client to gather posts with.</param>
		/// <param name="list">The list to add values to.</param>
		/// <returns></returns>
		Task GatherPostsAsync(IImageDownloaderClient client, List<IPost> list);
	}
}
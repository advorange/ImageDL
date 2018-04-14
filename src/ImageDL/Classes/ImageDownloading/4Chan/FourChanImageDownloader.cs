﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AdvorangesUtils;
using FChan.Library;
using ImageDL.Classes.SettingParsing;
using ImageDL.Interfaces;
using Model = ImageDL.Classes.ImageDownloading.FourChan.Models.FourChanPost;

namespace ImageDL.Classes.ImageDownloading.FourChan
{
	/// <summary>
	/// Downloads images from 4chan.
	/// </summary>
	public sealed class FourChanImageDownloader : ImageDownloader
	{
		/// <summary>
		/// The board to download images from.
		/// </summary>
		public string Board
		{
			get => _Board;
			set => _Board = value;
		}
		/// <summary>
		/// The thread to download images from.
		/// </summary>
		public int ThreadId
		{
			get => _ThreadId;
			set => _ThreadId = value;
		}

		private string _Board;
		private int _ThreadId;

		/// <summary>
		/// Creates an instance of <see cref="FourChanImageDownloader"/>.
		/// </summary>
		public FourChanImageDownloader() : base("4chan")
		{
			SettingParser.Add(new Setting<string>(new[] { nameof(Board), }, x => Board = x)
			{
				Description = "The board to download images from.",
			});
			SettingParser.Add(new Setting<int>(new[] { nameof(ThreadId), }, x => ThreadId = x)
			{
				Description = "The id of the thread to download images from. If left default, will download all threads.",
				IsOptional = true,
			});
		}

		/// <inheritdoc />
		protected override async Task GatherPostsAsync(IImageDownloaderClient client, List<IPost> list)
		{
			if (ThreadId > 0)
			{
				ProcessThread(list, await Chan.GetThreadAsync(Board, ThreadId).CAF(), ThreadId);
				return;
			}
			for (int i = 1; i < 10; ++i)
			{
				foreach (var thread in (await Chan.GetThreadPageAsync(Board, i).CAF()).Threads)
				{
					if (!ProcessThread(list, thread, thread.Posts[0].PostNumber))
					{
						return;
					}
				}
			}
		}

		/// <summary>
		/// Grabs all the images from the thread.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="thread"></param>
		/// <param name="threadId"></param>
		/// <returns></returns>
		private bool ProcessThread(List<IPost> list, Thread thread, int threadId)
		{
			for (int i = 0; i < thread.Posts.Count && list.Count < AmountOfPostsToGather; ++i)
			{
				var post = new Model(thread.Posts[i], threadId);
				//Return true because we want to stop processing this thread, but the other threads may still be processable
				if (post.CreatedAt < OldestAllowed)
				{
					return true;
				}
				else if (post.Post.IsStickied || post.Post.IsArchived || (post.Post.IsFileDeleted ?? false))
				{
					continue;
				}
				else if (!HasValidSize(null, post.Post.ImageWidth ?? -1, post.Post.ImageHeight ?? -1, out _))
				{
					continue;
				}
				else if (!Add(list, post))
				{
					return false;
				}
			}
			return list.Count < AmountOfPostsToGather;
		}
	}
}
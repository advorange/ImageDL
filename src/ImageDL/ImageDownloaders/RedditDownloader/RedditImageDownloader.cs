﻿using ImageDL.Classes;
using ImageDL.Utilities;
using RedditSharp;
using RedditSharp.Things;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ImageDL.ImageDownloaders.RedditDownloader
{
	/// <summary>
	/// Downloads images from reddit.
	/// </summary>
	public sealed class RedditImageDownloader : ImageDownloader<RedditImageDownloaderArguments, Post>
	{
		private Reddit _Reddit = new Reddit(new WebAgent(), false);

		protected override async Task<IEnumerable<Post>> GatherPostsAsync(RedditImageDownloaderArguments args)
		{
			try
			{
				var subreddit = await _Reddit.GetSubredditAsync(args.Subreddit);
				var validPosts = subreddit.Hot.Where(x => !x.IsStickied && !x.IsSelfPost && x.Score >= args.ScoreThreshold);
				return validPosts.Take(args.AmountToDownload);
			}
			catch (WebException e)
			{
				Console.WriteLine(e.Message);
			}
			return Enumerable.Empty<Post>();
		}
		protected override IEnumerable<Uri> GatherImages(Post post)
			=> UriUtils.GetImageUris(post.Url);
		protected override void WritePostToConsole(Post post, int count)
			=> Console.WriteLine($"[#{count}|\u2191{post.Score}] {post.Url}");
		protected override AnimatedContent StoreAnimatedContentLink(Post post, Uri uri)
			=> new AnimatedContent(uri, post.Score);
	}
}

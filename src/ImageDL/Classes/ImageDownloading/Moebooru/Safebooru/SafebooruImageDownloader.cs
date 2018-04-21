﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using AdvorangesUtils;
using ImageDL.Interfaces;
using ImageDL.Utilities;
using Newtonsoft.Json.Linq;
using Model = ImageDL.Classes.ImageDownloading.Moebooru.Safebooru.Models.SafebooruPost;

namespace ImageDL.Classes.ImageDownloading.Moebooru.Safebooru
{
	/// <summary>
	/// Downloads images from Safebooru.
	/// </summary>
	public sealed class SafebooruImageDownloader : MoebooruImageDownloader<Model>
	{
		/// <summary>
		/// Creates an instance of <see cref="SafebooruImageDownloader"/>.
		/// </summary>
		public SafebooruImageDownloader() : base("Safebooru", int.MaxValue, false) { }

		/// <inheritdoc />
		protected override Uri GenerateQuery(string tags, int page)
		{
			return GenerateSafebooruQuery(tags, page);
		}
		/// <inheritdoc />
		protected override List<Model> Parse(string text)
		{
			return ParseSafebooruPosts(text);
		}

		/// <summary>
		/// Generates a search uri.
		/// </summary>
		/// <param name="tags"></param>
		/// <param name="page"></param>
		/// <returns></returns>
		private static Uri GenerateSafebooruQuery(string tags, int page)
		{
			return new Uri($"https://safebooru.org/index.php" +
				$"?page=dapi" +
				$"&s=post" +
				$"&q=index" +
				$"&json=0" +
				$"&limit=100" +
				$"&tags={WebUtility.UrlEncode(tags)}" +
				$"&pid={page}");
		}
		/// <summary>
		/// Parses Safebooru posts from the supplied text.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private static List<Model> ParseSafebooruPosts(string text)
		{
			return JObject.Parse(JsonUtils.ConvertXmlToJson(text))["posts"]["post"].ToObject<List<Model>>();
		}
		/// <summary>
		/// Gets the post with the specified id.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public static async Task<Model> GetSafebooruPostAsync(IImageDownloaderClient client, string id)
		{
			var query = GenerateSafebooruQuery($"id:{id}", 0);
			var result = await client.GetText(() => client.GetReq(query)).CAF();
			return result.IsSuccess ? ParseSafebooruPosts(result.Value)[0] : null;
		}
		/// <summary>
		/// Gets images from the specified url.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="url"></param>
		/// <returns></returns>
		public static async Task<ImageResponse> GetSafebooruImagesAsync(IImageDownloaderClient client, Uri url)
		{
			var u = ImageDownloaderClient.RemoveQuery(url).ToString();
			if (u.IsImagePath())
			{
				return ImageResponse.FromUrl(new Uri(u));
			}
			var id = HttpUtility.ParseQueryString(url.Query)["id"];
			if (id != null && await GetSafebooruPostAsync(client, id).CAF() is Model post)
			{
				return await post.GetImagesAsync(client).CAF();
			}
			return ImageResponse.FromNotFound(url);
		}
	}
}
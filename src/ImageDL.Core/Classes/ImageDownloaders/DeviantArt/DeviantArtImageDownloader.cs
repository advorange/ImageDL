﻿using AdvorangesUtils;
using ImageDL.Classes.ImageScrapers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ImageDL.Classes.ImageDownloaders.DeviantArt
{
	/// <summary>
	/// Downloads images from DeviantArt.
	/// </summary>
	public sealed class DeviantArtImageDownloader : ImageDownloader<DeviantArtPost>
	{
		private const string SEARCH = "https://www.deviantartsupport.com/en/article/are-there-any-tricks-to-narrowing-down-a-search-on-deviantart";

		/// <summary>
		/// The client id to get the authorization token from.
		/// </summary>
		public string ClientId
		{
			get => _ClientId;
			set => NotifyPropertyChanged(_ClientId = value);
		}
		/// <summary>
		/// The redirection website. Must be a valid website supplied in 
		/// </summary>
		public string ClientSecret
		{
			get => _ClientSecret;
			set => NotifyPropertyChanged(_ClientSecret = value);
		}
		/// <summary>
		/// The username of the DeviantArt user to search.
		/// </summary>
		public string Username
		{
			get => _Username;
			set => NotifyPropertyChanged(_Username = value);
		}
		/// <summary>
		/// The tags to search for.
		/// </summary>
		public string TagString
		{
			get => _TagString;
			set => NotifyPropertyChanged(_TagString = value);
		}

		private string _ClientId;
		private string _ClientSecret;
		private string _Username;
		private string _TagString;

		/// <summary>
		/// Creates an image downloader for DeviantArt.
		/// </summary>
		public DeviantArtImageDownloader()
		{
			CommandLineParserOptions.Add($"id|{nameof(ClientId)}=", "the id of the client to get authentication from.", i => SetValue<string>(i, c => ClientId = c));
			CommandLineParserOptions.Add($"secret|{nameof(ClientSecret)}=", "the secret of the client to get authentication from.", i => SetValue<string>(i, c => ClientSecret = c));
			CommandLineParserOptions.Add($"user|{nameof(Username)}=", "the user gather images from.", i => SetValue<string>(i, c => Username = c));
			CommandLineParserOptions.Add($"tags|{nameof(TagString)}=", $"the tags to search for. For additional help, visit {SEARCH}", i => SetValue<string>(i, c => TagString = c));

			ClientId = null;
			ClientSecret = null;
			Username = null;
			TagString = null;
		}

		/// <inheritdoc />
		protected override async Task<List<DeviantArtPost>> GatherPostsAsync()
		{
			var validPosts = new List<DeviantArtPost>();
			try
			{
				string token = null;
				if (ClientId != null && ClientSecret != null)
				{
					var request = $"https://www.deviantart.com/oauth2/token" +
						$"?grant_type=client_credentials" +
						$"&client_id={ClientId}" +
						$"&client_secret={ClientSecret}";

					using (var resp = await Client.SendWithRefererAsync(new Uri(request), HttpMethod.Get).CAF())
					{
						if (resp.IsSuccessStatusCode)
						{
							token = JObject.Parse(await resp.Content.ReadAsStringAsync().CAF())["access_token"].ToObject<string>();
						}
					}
				}

				if (!String.IsNullOrWhiteSpace(token))
				{
					Client.UpdateAPIKey(token, TimeSpan.FromHours(1));
					validPosts = (await GetPostsThroughApi(token).CAF()).Select(x => new DeviantArtPost(x)).ToList();
				}
				else
				{
					validPosts = (await GetPostsThroughScraping().CAF()).Select(x => new DeviantArtPost(x)).ToList();
				}
			}
			catch (WebException we) when (we.Message.Contains("403")) { } //Eat this error due to not being able to know when to stop
			catch (Exception e)
			{
				e.Write();
			}
			finally
			{
				Console.WriteLine($"Finished gathering DeviantArt posts.");
				Console.WriteLine();
			}
			return validPosts.GroupBy(x => x.Source).Select(x => x.First()).OrderByDescending(x => x.Favorites).ToList();
		}
		/// <inheritdoc />
		protected override void WritePostToConsole(DeviantArtPost post, int count)
		{
			var postHasScore = post.Favorites > 0 ? $"|\u2191{post.Favorites}" : "";
			Console.WriteLine($"[#{count}{postHasScore}] {post.Source}");
		}
		/// <inheritdoc />
		protected override FileInfo GenerateFileInfo(DeviantArtPost post, Uri uri)
		{
			var extension = Path.GetExtension(uri.LocalPath);
			var name = $"{post.PostId}_{Path.GetFileNameWithoutExtension(uri.LocalPath)}";
			return GenerateFileInfo(Directory, name, extension);
		}
		/// <inheritdoc />
		protected override async Task<ScrapeResult> GatherImagesAsync(DeviantArtPost post)
		{
			return await Client.ScrapeImagesAsync(new Uri(post.Source)).CAF();
		}
		/// <inheritdoc />
		protected override ContentLink CreateContentLink(DeviantArtPost post, Uri uri, string reason)
		{
			//If favorites are there then use that, otherwise just use the post id
			return new ContentLink(uri, post.Favorites < 0 ? post.PostId : post.Favorites, reason);
		}

		private string GenerateQuery()
		{
			var query = WebUtility.UrlEncode(TagString);
			if (MaxDaysOld > 0)
			{
				query += $"+max_age:{MaxDaysOld}d";
			}
			if (!String.IsNullOrWhiteSpace(Username))
			{
				query += $"+by:{WebUtility.UrlEncode(Username)}";
			}
			return query.Trim('+');
		}
		private async Task<List<ScrapedDeviantArtPost>> GetPostsThroughScraping()
		{
			var validPosts = new List<ScrapedDeviantArtPost>();
			for (int i = 0; validPosts.Count < AmountToDownload;)
			{
				var search = $"https://www.deviantart.com/newest/" +
					$"?offset={i}" +
					$"&q={GenerateQuery()}";

				var html = await Client.GetMainTextAndRetryIfRateLimitedAsync(new Uri(search), TimeSpan.FromSeconds(1)).CAF();
				var jsonStart = "window.__pageload =";
				var jsonStartIndex = html.IndexOf(jsonStart) + jsonStart.Length;
				var jsonEnd = "}}}</script>";
				var jsonEndIndex = html.IndexOf(jsonEnd) + 3;

				//Now we have all the json, but we only want the artwork json so we have to parse that manually
				var finished = false;
				var posts = JObject.Parse(html.Substring(jsonStartIndex, jsonEndIndex - jsonStartIndex).Trim())["metadata"]
					.Select(x =>
					{
						try
						{
							return x.First.ToObject<ScrapedDeviantArtPost>();
						}
						catch (JsonSerializationException) //Ignore any serialization exceptions, just don't include them
						{
							return null;
						}
					}).Where(x => x != null).ToList();
				foreach (var post in posts)
				{
					if (!FitsSizeRequirements(null, post.Width, post.Height, out _)) //Can't check score/favorites when scraping
					{
						continue;
					}

					validPosts.Add(post);
					if (validPosts.Count == AmountToDownload)
					{
						finished = true;
						break;
					}
					else if (validPosts.Count % 25 == 0)
					{
						Console.WriteLine($"{validPosts.Count} DeviantArt posts found.");
					}
				}

				//24 is a full page, but for some reason only 22 can be gotten usually
				if (finished || posts.Count < 22)
				{
					break;
				}
				i += posts.Count;
			}
			return validPosts;
		}
		private async Task<List<ApiDeviantArtResults.ApiDeviantArtPost>> GetPostsThroughApi(string token)
		{
			var validPosts = new List<ApiDeviantArtResults.ApiDeviantArtPost>();
			for (int i = 0; validPosts.Count < AmountToDownload;)
			{
				var search = $"https://www.deviantart.com/api/v1/oauth2/browse/newest" +
					$"?offset={i}" +
					$"&q={GenerateQuery()}" +
					$"&access_token={token}";

				var json = await Client.GetMainTextAndRetryIfRateLimitedAsync(new Uri(search), TimeSpan.FromSeconds(1)).CAF();

				//Deserialize the Json and look through all the posts
				var finished = false;
				var result = JsonConvert.DeserializeObject<ApiDeviantArtResults>(json);
				foreach (var post in result.Results)
				{
					if (post.Content == null) //Is a journal or something like that
					{
						continue;
					}
					else if (!FitsSizeRequirements(null, post.Content.Width, post.Content.Height, out _) || post.Stats.Favorites < MinScore)
					{
						continue;
					}

					validPosts.Add(post);
					if (validPosts.Count == AmountToDownload)
					{
						finished = true;
						break;
					}
					else if (validPosts.Count % 25 == 0)
					{
						Console.WriteLine($"{validPosts.Count} DeviantArt posts found.");
					}
				}

				//Break out if finished or no more are left
				if (finished || !result.HasMore)
				{
					break;
				}
				i += result.Results.Count;
			}
			return validPosts;
		}
	}
}

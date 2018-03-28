﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageDL.Classes.ImageDownloading.Danbooru
{
	/// <summary>
	/// Json model for a danbooru post.
	/// </summary>
	public class DanbooruPost
	{
#pragma warning disable 1591 //Disabled since most of these are self explanatory and this is a glorified Json model
		[JsonProperty("id")]
		public readonly int Id;
		[JsonProperty("uploader_id")]
		public readonly int UploaderId;
		[JsonProperty("parent_id")]
		public readonly int? ParentId;
		[JsonProperty("approver_id")]
		public readonly int? ApproverId;
		[JsonProperty("pixiv_id")]
		public readonly int? PixivId;

		[JsonProperty("score")]
		public readonly int Score;
		[JsonProperty("up_score")]
		public readonly int UpScore;
		[JsonProperty("down_score")]
		public readonly int DownScore;
		[JsonProperty("fav_count")]
		public readonly int FavCount;

		[JsonProperty("is_note_locked")]
		public readonly bool IsNoteLocked;
		[JsonProperty("is_rating_locked")]
		public readonly bool IsRatingLocked;
		[JsonProperty("is_status_locked")]
		public readonly bool IsStatusLocked;
		[JsonProperty("is_pending")]
		public readonly bool IsPending;
		[JsonProperty("is_flagged")]
		public readonly bool IsFlagged;
		[JsonProperty("is_deleted")]
		public readonly bool IsDeleted;
		[JsonProperty("is_banned")]
		public readonly bool IsBanned;

		[JsonProperty("md5")]
		public readonly string Md5;
		[JsonProperty("file_ext")]
		public readonly string FileExt;
		[JsonProperty("file_size")]
		public readonly long FileSize;
		[JsonProperty("image_width")]
		public readonly int ImageWidth;
		[JsonProperty("image_height")]
		public readonly int ImageHeight;
		[JsonProperty("file_url")]
		public readonly string FileUrl;
		[JsonProperty("large_file_url")]
		public readonly string LargeFileUrl;
		[JsonProperty("preview_file_url")]
		public readonly string PreviewFileUrl;
		[JsonProperty("has_large")]
		public readonly bool HasLarge;

		[JsonProperty("has_children")]
		public readonly bool HasChildren;
		[JsonProperty("has_active_children")]
		public readonly bool HasActiveChildren;
		[JsonProperty("has_visible_children")]
		public readonly bool HasVisibleChildren;
		[JsonProperty("children_ids")]
		public readonly string ChildrenIdsString;

		[JsonProperty("created_at")]
		public readonly DateTime CreatedAt;
		[JsonProperty("last_comment_bumped_at")]
		public readonly DateTime? LastCommentBumpedAt;
		[JsonProperty("last_noted_at")]
		public readonly DateTime? LastNotedAt;
		[JsonProperty("updated_at")]
		public readonly DateTime? UpdatedAt;
		[JsonProperty("last_commented_at")]
		public readonly DateTime? LastCommentedAt;

		[JsonProperty("source")]
		public readonly string Source;
		[JsonProperty("uploader_name")]
		public readonly string UploaderName;
		[JsonProperty("rating")]
		public readonly char Rating;
		[JsonProperty("bit_flags")]
		public readonly ulong BitFlags; //Not sure if this is the correct type
		[JsonProperty("fav_string")]
		public readonly string FavString;
		[JsonProperty("pool_string")]
		public readonly string PoolString;
		[JsonProperty("keeper_data")]
		public readonly Dictionary<string, int> KeeperData; //Not sure if this is the correct type

		#region Tags
		[JsonProperty("tag_string")]
		public readonly string TagString;
		[JsonProperty("tag_string_general")]
		public readonly string TagStringGeneral;
		[JsonProperty("tag_string_character")]
		public readonly string TagStringCharacter;
		[JsonProperty("tag_string_copyright")]
		public readonly string TagStringCopyright;
		[JsonProperty("tag_string_artist")]
		public readonly string TagStringArtist;
		[JsonProperty("tag_string_meta")]
		public readonly string TagStringMeta;

		[JsonProperty("tag_count")]
		public readonly int TagCount;
		[JsonProperty("tag_count_general")]
		public readonly int TagCountGeneral;
		[JsonProperty("tag_count_character")]
		public readonly int TagCountCharacter;
		[JsonProperty("tag_count_copyright")]
		public readonly int TagCountCopyright;
		[JsonProperty("tag_count_artist")]
		public readonly int TagCountArtist;
		[JsonProperty("tag_count_meta")]
		public readonly int TagCountMeta;

		public string[] this[TagType type]
		{
			get
			{
				switch (type)
				{
					case TagType.All:
						return TagString.Split(' ');
					case TagType.General:
						return TagStringGeneral.Split(' ');
					case TagType.Character:
						return TagStringCharacter.Split(' ');
					case TagType.Copyright:
						return TagStringCopyright.Split(' ');
					case TagType.Artist:
						return TagStringArtist.Split(' ');
					case TagType.Meta:
						return TagStringMeta.Split(' ');
					default:
						throw new ArgumentException("Invalid type tag type supplied.", nameof(type));
				}
			}
		}
		#endregion

		[JsonIgnore]
		public int[] ChildrenIds => String.IsNullOrWhiteSpace(ChildrenIdsString)
			? new int[0] : ChildrenIdsString.Split(' ').Select(x => Convert.ToInt32(x)).ToArray();
		[JsonIgnore]
		public int[] Favorites => String.IsNullOrWhiteSpace(FavString)
			? new int[0] : FavString.Split(' ').Select(x => Convert.ToInt32(x.Replace("fav:", ""))).ToArray();
		[JsonIgnore]
		public string[] Pools => String.IsNullOrWhiteSpace(PoolString)
			? new string[0] : PoolString.Split(' ').Select(x => x.Replace("pool:", "")).ToArray();
#pragma warning restore 1591

		/// <summary>
		/// The type of tags to get.
		/// </summary>
		public enum TagType
		{
			/// <summary>
			/// Every tag that the image has on it. General, meta, etc.
			/// </summary>
			All,
			/// <summary>
			/// Tags for what the character is doing or looking like.
			/// </summary>
			General,
			/// <summary>
			/// Tags for who is in the image.
			/// </summary>
			Character,
			/// <summary>
			/// Tags for who owns the image.
			/// </summary>
			Copyright,
			/// <summary>
			/// Tags for who made the image.
			/// </summary>
			Artist,
			/// <summary>
			/// Tags about the image file. Resolution, official, etc.
			/// </summary>
			Meta,
		}
	}
}
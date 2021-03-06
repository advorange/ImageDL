﻿using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageDL.Classes.ImageDownloading.Weibo.Models
{
	/// <summary>
	/// When gathering from a user these are nested inside an object, but when gathered from a post they're a string.
	/// </summary>
	internal class WeiboPagePictureConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType) => true;

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.TokenType == JsonToken.StartObject)
			{
				return JObject.Load(reader)["url"].ToObject<Uri>();
			}
			else
			{
				return serializer.Deserialize(reader, typeof(Uri));
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
	}
}
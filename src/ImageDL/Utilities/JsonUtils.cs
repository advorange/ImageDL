﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System.Net;

namespace ImageDL.Utilities
{
	/// <summary>
	/// Utilities for Json.
	/// </summary>
	public static class JsonUtils
	{
		/// <summary>
		/// Returns a json string, with no @'s in front of names.
		/// </summary>
		/// <param name="xml"></param>
		/// <returns></returns>
		public static string ConvertXmlToJson(string xml)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);
			var builder = new StringBuilder();
			using (var writer = new StringWriter(builder))
			{
				JsonSerializer.Create().Serialize(new CustomJsonWriter(writer), doc);
				return builder.ToString();
			}
		}
		/// <summary>
		/// Returns a json string.
		/// </summary>
		/// <param name="html"></param>
		/// <returns></returns>
		public static string ConvertHtmlToJson(string html)
		{
			var doc = new HtmlDocument();
			doc.LoadHtml(html);
			return ConvertHtmlNodeToJObject(doc.DocumentNode).ToString();
		}
		/// <summary>
		/// Returns a json string created from the supplied node.
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		public static JObject ConvertHtmlNodeToJObject(HtmlNode node)
		{
			var jObj = new JObject();
			foreach (var attribute in node.Attributes)
			{
				jObj.Add(attribute.Name, WebUtility.HtmlDecode(attribute.Value));
			}
			var text = node.InnerText.Trim();
			if (!String.IsNullOrWhiteSpace(text))
			{
				jObj.Add("inner_text", WebUtility.HtmlDecode(text));
			}
			var childJTokens = node.ChildNodes.Select(x => ConvertHtmlNodeToJObject(x)).Where(x => x.HasValues);
			if (childJTokens.Any())
			{
				jObj.Add("children", new JArray(childJTokens));
			}
			return jObj;
		}
	}

	//Source: https://stackoverflow.com/a/43485727
	internal class CustomJsonWriter : JsonTextWriter
	{
		public CustomJsonWriter(TextWriter writer) : base(writer) { }

		public override void WritePropertyName(string name)
		{
			if (name.StartsWith("@") || name.StartsWith("#"))
			{
				base.WritePropertyName(name.Substring(1));
			}
			else
			{
				base.WritePropertyName(name);
			}
		}
	}
}
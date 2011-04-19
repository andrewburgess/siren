using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LastFM
{
	public class AlbumInfo
	{
		public Guid Id { get; private set; }
		public string Name { get; private set; }
		public string Artist { get; private set;}
		public Guid MBID { get; private set; }
		public string Url { get; private set; }
		public Dictionary<ImageSize, string> Images { get; private set; }
		public long Listeners { get; private set; }
		public long Plays { get; private set; }
		public string Summary { get; private set; }
		public string Bio { get; private set; }
		public DateTime ReleaseDate { get; private set; }

		public AlbumInfo(Guid id, string xml)
		{
			Id = id;
			Images = new Dictionary<ImageSize, string>();
			var x = XDocument.Parse(xml);
			Name = (from a in x.Descendants("album") select a.Element("name")).First().Value;
			Artist = (from a in x.Descendants("album") select a.Element("artist")).First().Value;
			Guid mbid;
			if (Guid.TryParse((from a in x.Descendants("album") select a.Element("mbid")).First().Value, out mbid))
				MBID = mbid;
			Url = (from a in x.Descendants("album") select a.Element("url")).First().Value;

			var images = (from a in x.Descendants("album") select a.Elements("image")).First();
			foreach (var img in images)
			{
				var s = img.Attribute("size").Value;
				var url = img.Value;
				ImageSize size;
				switch (s)
				{
					case "small":
						size = ImageSize.Small;
						break;
					case "medium":
						size = ImageSize.Medium;
						break;
					case "large":
						size = ImageSize.Large;
						break;
					case "extralarge":
						size = ImageSize.ExtraLarge;
						break;
					case "mega":
						size = ImageSize.Mega;
						break;
					default:
						size = ImageSize.Medium;
						break;
				}

				Images.Add(size, url);
			}

			Listeners = long.Parse((from a in x.Descendants("album") select a.Element("listeners")).First().Value);
			Plays = long.Parse((from a in x.Descendants("album") select a.Element("playcount")).First().Value);

			Summary = "";
			Bio = "";
			var sum = (from a in x.Descendants("wiki") select a.Element("summary"));
			var bio = (from a in x.Descendants("wiki") select a.Element("content"));

			if (sum.Count() > 0)
				Summary = sum.First().Value;
			if (bio.Count() > 0)
				Bio = bio.First().Value;

			Summary = Summary.Replace("<![CDATA[", "").Replace("]]>", "");
			Summary = HtmlRemoval.StripTagsRegexCompiled(Summary);
			Summary = Summary.Replace("&quot;", "\"").Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");

			Bio = Bio.Replace("<![CDATA[", "").Replace("]]>", "");
			Bio = HtmlRemoval.StripTagsRegexCompiled(Bio);
			Bio = Bio.Replace("&quot;", "\"").Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");

			DateTime release;
			if (!DateTime.TryParse((from a in x.Descendants("album") select a.Element("releasedate")).First().Value, out release))
				release = DateTime.MinValue;

			ReleaseDate = release;
		}
	}
}
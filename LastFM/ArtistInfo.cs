using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LastFM
{
	public class ArtistInfo
	{
		public Guid Id { get; private set; }
		public string Name { get; private set; }
		public Guid MBID { get; private set; }
		public string Url { get; private set; }
		public List<KeyValuePair<string, string>> Images { get; private set; }
		public long Listeners { get; private set; }
		public long Plays { get; private set; }
		public string Summary { get; private set; }
		public string Bio { get; private set; }

		public ArtistInfo(Guid id, string xml)
		{
			Id = id;
			Images = new List<KeyValuePair<string, string>>();
			var x = XDocument.Parse(xml);
			Name = (from a in x.Descendants("artist") select a.Element("name")).First().Value;
			Guid mbid;
			if (Guid.TryParse((from a in x.Descendants("artist") select a.Element("mbid")).First().Value, out mbid))
				MBID = mbid;
			Url = (from a in x.Descendants("artist") select a.Element("url")).First().Value;

			var images = (from a in x.Descendants("artist") select a.Elements("image")).First();

			foreach (var img in images)
			{
				var size = img.Attribute("size").Value;
				var url = img.Value;
				Images.Add(new KeyValuePair<string, string>(size, url));
			}

			Listeners = long.Parse((from a in x.Descendants("stats") select a.Element("listeners")).First().Value);
			Plays = long.Parse((from a in x.Descendants("stats") select a.Element("playcount")).First().Value);

			Summary = (from a in x.Descendants("bio") select a.Element("summary")).First().Value;
			Bio = (from a in x.Descendants("bio") select a.Element("content")).First().Value;

			Summary = Summary.Replace("<![CDATA[", "").Replace("]]>", "");
			Summary = HtmlRemoval.StripTagsRegexCompiled(Summary);
			Summary = Summary.Replace("&quot;", "\"").Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");

			Bio = Bio.Replace("<![CDATA[", "").Replace("]]>", "");
			Bio = HtmlRemoval.StripTagsRegexCompiled(Bio);
			Bio = Bio.Replace("&quot;", "\"").Replace("&amp;", "&").Replace("&gt;", ">").Replace("&lt;", "<");
		}
	}
}
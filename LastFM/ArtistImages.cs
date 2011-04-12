using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LastFM
{
	public class ArtistImages
	{
		public List<KeyValuePair<ImageSize, string >> ImageUrls { get; private set; }

		public ArtistImages(string xml)
		{
			ImageUrls = new List<KeyValuePair<ImageSize, string>>();
			var x = XDocument.Parse(xml);
			var images = from a in x.Descendants("image") select a;

			foreach (var img in images)
			{
				var sizes = from s in img.Descendants("size") select s;

				foreach (var size in sizes)
				{
					if (size.Attribute("name").Value == "largesquare")
					{
						ImageUrls.Add(new KeyValuePair<ImageSize, string>(ImageSize.LargeSquare, size.Value));
					}
					else if (size.Attribute("name").Value == "extralarge")
					{
						ImageUrls.Add(new KeyValuePair<ImageSize, string>(ImageSize.ExtraLarge, size.Value));
					}
				}
			}
		}
	}
}

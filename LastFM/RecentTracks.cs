using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace LastFM
{
	public class TrackPlay
	{
		public string Artist { get; private set; }
		public string TrackName { get; private set; }
		public DateTime DatePlayed { get; private set; }

		public TrackPlay(string artist, string trackName, DateTime datePlayed)
		{
			Artist = artist;
			TrackName = trackName;
			DatePlayed = datePlayed;
		}
	}

	public class RecentTracks
	{
		public int Page { get; private set; }
		public int PerPage { get; private set; }
		public int TotalPages { get; private set; }
		public List<TrackPlay> TrackPlays { get; private set; }

		public RecentTracks(string data)
		{
			TrackPlays = new List<TrackPlay>();
			var x = XDocument.Parse(data);

			Page = int.Parse(x.Descendants("recenttracks").First().Attribute("page").Value);
			PerPage = int.Parse(x.Descendants("recenttracks").First().Attribute("perPage").Value);
			TotalPages = int.Parse(x.Descendants("recenttracks").First().Attribute("totalPages").Value);

			var t = from a in x.Descendants("recenttracks")
			        select a.Descendants("track");

			foreach (var track in t.First())
			{
				if (track.Attribute("nowplaying") == null)
				{
					TrackPlays.Add(new TrackPlay(track.Descendants("artist").First().Value, track.Descendants("name").First().Value,
					                             DateTime.Parse(track.Descendants("date").First().Value)));
				}
			}
		}
	}
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace LastFM
{
	public static class API
	{
		private const string API_KEY = "21edbb30193dc36e6fb21cc57b1d8e18";
		private const string LASTFM_URL = "http://ws.audioscrobbler.com/2.0/?method=";
		private const int DELAY = 1000;

		private static DateTime LastRead = DateTime.Now;

		private static string DownloadData(string url)
		{
			var client = new WebClient();

			while (LastRead.AddMilliseconds(DELAY) > DateTime.Now) { }

			LastRead = DateTime.Now;

			return client.DownloadString(url);
		}

		public static class Artist
		{
			public static ArtistInfo GetInfo(Guid id, string artist, bool autoCorrect = true)
			{
				var url = LASTFM_URL +
				          string.Format("artist.getinfo&artist={0}&autocorrect={1}&api_key={2}", HttpUtility.UrlEncode(artist), autoCorrect ? 1 : 0,
				                        API_KEY);
				var data = DownloadData(url);
				return new ArtistInfo(id, data);
			}
		}
	}
}

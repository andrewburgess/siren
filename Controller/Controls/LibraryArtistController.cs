using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Controller.Interfaces.Controls;
using LastFM;
using Model;

namespace Controller.Controls
{
	public class LibraryArtistController
	{
		private ILibraryArtistControl View { get; set; }
		private static Repository Repository { get; set; }

		public LibraryArtistController(ILibraryArtistControl view)
		{
			View = view;

			if (Repository == null)
				Repository = DataAccessContext.GetRepository();
		}

		public void InitializeView()
		{
			var artist = View.Artist;
			View.SetArtistName(artist.Name);
			View.SetArtistDescription(artist.Summary);
			View.SetAlbumCount(artist.Albums.Count);
			View.SetTrackCount(artist.Tracks.Count);

			var playCount = (from x in Repository.Tracks
			                 join y in Repository.Plays on x.Id equals y.TrackId
			                 where x.ArtistId == artist.Id
			                 select y).Count();
			View.SetPlayCount(playCount);

			var score = (from x in Repository.Tracks
			             where x.ArtistId == artist.Id
			             select x.Score).Average();
			View.SetScore(score);

			var img = (from i in Repository.Images
			           where i.LinkedId == artist.Id
					   where i.Size == (int)ImageSize.Mega
			           select i).FirstOrDefault();
			if (img != null)
				View.SetImage(img);
		}
	}
}

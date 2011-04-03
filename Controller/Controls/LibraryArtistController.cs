using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Controller.Interfaces.Controls;
using Model;

namespace Controller.Controls
{
	public class LibraryArtistController
	{
		private ILibraryArtistControl View { get; set; }
		private Repository Repository { get; set; }

		public LibraryArtistController(ILibraryArtistControl view)
		{
			View = view;
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
		}
	}
}

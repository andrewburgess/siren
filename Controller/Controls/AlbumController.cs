using System;
using System.Linq;
using Controller.Interfaces.Controls;
using Controller.Utilities;
using Model;

namespace Controller.Controls
{
	public class AlbumController
	{
		private IAlbumControl View { get; set; }
		private Repository Repository { get; set; }

		public AlbumController(IAlbumControl view)
		{
			View = view;
			Repository = DataAccessContext.GetRepository();
		}

		public void InitializeView()
		{
			var a = View.Album;
			var tracks = Repository.Tracks.Where(x => x.AlbumId == a.Id).OrderBy(x => x.TrackNumber);
			var img = Repository.Images.Where(x => x.LinkedId == a.Id);

			View.SetName(a.Name);
			View.SetTracks(tracks);
			if (img.Count() > 0)
				View.SetImage(ImageUtilities.ImageFromBuffer(img.First().ImageData.ToArray()));
		}
	}
}
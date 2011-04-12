using System.Linq;
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

			var img = (from i in Repository.Images
			           where i.LinkedId == artist.Id
					   where i.Size == (int)ImageSize.LargeSquare
			           select i).FirstOrDefault();
			if (img != null)
				View.SetImage(img);
		}
	}
}

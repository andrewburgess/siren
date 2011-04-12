using System;
using System.Linq;
using Controller.Interfaces.Controls;
using LastFM;
using Model;

namespace Controller.Controls
{
	public class ArtistDetailController
	{
		private IArtistDetailControl View { get; set; }
		private Repository Repository { get; set; }

		public ArtistDetailController(IArtistDetailControl view )
		{
			View = view;
			Repository = DataAccessContext.GetRepository();
		}

		public void InitializeView()
		{
			var images = from i in Repository.Images
			             where i.LinkedId == View.Artist.Id
			             where i.Size == (int) ImageSize.ExtraLarge
			             select i;

			View.SetImages(images);
		}
	}
}
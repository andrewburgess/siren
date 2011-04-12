using System;
using System.ComponentModel;
using Controller.Interfaces;
using Controller.Interfaces.Controls;
using Model;

namespace Controller
{

	public class SirenController
	{
		private IMainWindowView View { get; set; }
		private ILibraryControl LibraryControl { get; set; }
		private Repository Repository { get; set; }

		private static SirenController Instance { get; set; }

		public SirenController(IMainWindowView view)
		{
			View = view;
			Instance = this;
			
			Repository = DataAccessContext.GetRepository();
		}

		public void InitializeView()
		{
			View.LoadLibraryView();

			LibraryControl = View.LibraryControl;
		}

		public static SirenController GetInstance()
		{
			return Instance;
		}

		public void LastFMUpdate(object sender, ProgressChangedEventArgs e)
		{
			if (e.UserState != null)
			{
				if (e.UserState is Artist)
				{
					LibraryControl.UpdateArtist((Artist) e.UserState);
				}
			}
		}

		public void ShowArtist(Artist artist)
		{
			View.ShowArtistPanel(artist);
		}

		public void Back()
		{
			View.GoBack();
		}
	}
}

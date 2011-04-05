using System;
using System.Linq;
using Controller.Interfaces.Controls;
using Model;

namespace Controller.Controls
{
	public class LibraryControlController
	{
		private ILibraryControl View { get; set; }
		private Repository Repository { get; set; }

		public LibraryControlController(ILibraryControl view)
		{
			View = view;
		}

		public void InitializeView()
		{
			Repository = DataAccessContext.GetRepository();
			View.LibraryItems = Repository.Artists.Where(x => x.Tracks.Count() > 0);
		}
	}
}
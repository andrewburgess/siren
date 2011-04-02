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
			Repository = DataAccessContext.GetRepository();
		}

		public void InitializeView()
		{
			View.LibraryItems = Repository.Artists.Where(x => x.Tracks.Count() > 0);
		}
	}
}
using Controller.Interfaces.Controls;
using Model;

namespace Controller.Interfaces
{
	public interface IMainWindowView
	{
		void LoadLibraryView();
		ILibraryControl LibraryControl { get; }
		void ShowArtistPanel(Artist artist);
		void GoBack();
	}
}

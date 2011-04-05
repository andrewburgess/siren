using Controller.Interfaces.Controls;

namespace Controller.Interfaces
{
	public interface IMainWindowView
	{
		void LoadLibraryView();
		ILibraryControl LibraryControl { get; }
	}
}

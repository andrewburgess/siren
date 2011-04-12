using Model;

namespace Controller.Interfaces.Controls
{
	public interface ILibraryArtistControl
	{
		Artist Artist { get; set; }
		void SetArtistName(string name);
		void SetImage(Image image);
	}
}
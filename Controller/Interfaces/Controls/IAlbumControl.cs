using System.Linq;
using System.Windows.Media.Imaging;
using Controller.Controls;
using Model;

namespace Controller.Interfaces.Controls
{
	public interface IAlbumControl
	{
		Album Album { get; }
		void SetTracks(IQueryable<Track> tracks);
		void SetImage(BitmapImage image);
		void SetName(string name);
	}
}
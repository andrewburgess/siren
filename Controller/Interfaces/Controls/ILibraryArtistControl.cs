using Model;

namespace Controller.Interfaces.Controls
{
	public interface ILibraryArtistControl
	{
		Artist Artist { get; set; }
		void SetArtistName(string name);
		void SetArtistDescription(string description);
		void SetAlbumCount(int albumCount);
		void SetTrackCount(int trackCount);
		void SetPlayCount(int playCount);
		void SetScore(double score);
	}
}
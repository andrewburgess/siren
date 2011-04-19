using Model;

namespace Controller.Interfaces.Controls
{
	public interface ITrackControl
	{
		Track Track { get; }
		void SetTrackNumber(int trackNumber);
		void SetTrackName(string name);
		void SetTrackLength(string length);
		void SetTrackPlays(int count);
		void SetTrackScore(int score);
	}
}
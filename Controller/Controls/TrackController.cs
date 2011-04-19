using System;
using Controller.Interfaces.Controls;

namespace Controller.Controls
{
	public class TrackController
	{
		private ITrackControl View { get; set; }

		public TrackController(ITrackControl view)
		{
			View = view;
		}

		public void InitializeView()
		{
			var track = View.Track;

			View.SetTrackNumber(track.TrackNumber);
			View.SetTrackName(track.Name);
			var ts = TimeSpan.FromSeconds(track.Duration);
			View.SetTrackLength(ts.ToString(@"m\:ss"));
			View.SetTrackPlays(track.Plays.Count);
			View.SetTrackScore(track.Score);
		}
	}
}
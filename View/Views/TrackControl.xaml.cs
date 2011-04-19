using System;
using System.Windows;
using System.Windows.Controls;
using Controller.Controls;
using Controller.Interfaces.Controls;
using Model;

namespace View.Views
{
	/// <summary>
	/// Interaction logic for TrackControl.xaml
	/// </summary>
	public partial class TrackControl : ITrackControl
	{
		private TrackController Controller { get; set; }

		public TrackControl(Track track)
		{
			InitializeComponent();

			Track = track;
			Controller = new TrackController(this);
		}

		private void TrackControlLoaded(object sender, RoutedEventArgs e)
		{
			Controller.InitializeView();
		}

		public Track Track { get; set; }
		public void SetTrackNumber(int trackNumber)
		{
			TrackNumber.Content = trackNumber + ".";
		}

		public void SetTrackName(string name)
		{
			TrackName.Content = name;
		}

		public void SetTrackLength(string length)
		{
			TrackLength.Content = length;
		}

		public void SetTrackPlays(int count)
		{
			TrackPlays.Content = count + (count == 1 ? " play" : " plays");
		}

		public void SetTrackScore(int score)
		{
			TrackScore.Content = score + "%";
		}
	}
}
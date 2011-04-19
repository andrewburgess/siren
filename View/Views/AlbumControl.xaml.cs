using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Controller.Controls;
using Controller.Interfaces.Controls;
using Model;

namespace View.Views
{
	/// <summary>
	/// Interaction logic for AlbumControl.xaml
	/// </summary>
	public partial class AlbumControl : IAlbumControl
	{
		private AlbumController Controller { get; set; }

		public AlbumControl(Album album)
		{
			InitializeComponent();
			Album = album;

			Controller = new AlbumController(this);
		}

		private void AlbumControlLoaded(object sender, RoutedEventArgs e)
		{
			Controller.InitializeView();
		}

		public Album Album { get; set; }

		public void SetTracks(IQueryable<Track> tracks)
		{
			foreach (var t in tracks)
			{
				Tracks.Children.Add(new TrackControl(t));
			}
		}

		public void SetImage(BitmapImage image)
		{
			AlbumCover.Source = image;
		}

		public void SetName(string name)
		{
			AlbumName.Content = name;
		}
	}
}
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Controller.Controls;
using Controller.Interfaces.Controls;
using Model;
using Image = System.Windows.Controls.Image;

namespace View.Views
{
	/// <summary>
	/// Interaction logic for LibraryArtist.xaml
	/// </summary>
	public partial class LibraryArtist : ILibraryArtistControl
	{
		private LibraryArtistController Controller { get; set; }

		public Artist Artist { get; set; }
		
		public LibraryArtist(Artist artist)
		{
			InitializeComponent();
			Artist = artist;

			Controller = new LibraryArtistController(this);
			Controller.InitializeView();

			/*var x = (Image)LayoutRoot.Resources["img"];
			var y = new BitmapImage();
			y.BeginInit();
			y.UriSource = new Uri(@"test2.png", UriKind.Absolute);
			y.EndInit();
			x.Source = y;

			LayoutRoot.Background = CreateBrush();
			var img = new Image();
			img.Source = y;
			ArtistData.Children.Add(img);*/
		}

		#region ILibraryArtistControl Implementation
		public void SetArtistName(string name)
		{
			ArtistName.Text = name;
		}

		public void SetArtistDescription(string description)
		{
			ArtistDescription.Text = description;
		}

		public void SetAlbumCount(int albumCount)
		{
			AlbumCount.Content = string.Format("Albums: {0}", albumCount);
		}

		public void SetTrackCount(int trackCount)
		{
			TrackCount.Content = string.Format("Tracks: {0}", trackCount);
		}

		public void SetPlayCount(int playCount)
		{
			PlayCount.Content = string.Format("Played {0:d} times", playCount);
		}

		public void SetScore(double score)
		{
			Score.Content = string.Format("Score: {0}%", score);
		}
		#endregion

		private VisualBrush CreateBrush()
		{
			var brush = new VisualBrush
			            	{
			            		Visual = (Image) LayoutRoot.Resources["img"],
			            		AlignmentX = AlignmentX.Left,
			            		AlignmentY = AlignmentY.Center,
			            		Stretch = Stretch.Uniform
			            	};
			return brush;
		}
	}
}
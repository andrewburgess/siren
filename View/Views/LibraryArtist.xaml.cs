using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Model;
using Image = System.Windows.Controls.Image;

namespace View.Views
{
	/// <summary>
	/// Interaction logic for LibraryArtist.xaml
	/// </summary>
	public partial class LibraryArtist
	{
		public Artist Artist { get; set; }
		public LibraryArtist(Artist artist)
		{
			MouseUp += delegate { MessageBox.Show(ActualWidth.ToString()); };
			InitializeComponent();

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

			Artist = artist;

			ArtistName.Text = artist.Name;
			//ArtistDescription.Text = artist.Bio;
			AlbumCount.Content = string.Format("Albums: {0}", artist.Albums.Count);
			TrackCount.Content = string.Format("Tracks: {0}", artist.Tracks.Count);
		}

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
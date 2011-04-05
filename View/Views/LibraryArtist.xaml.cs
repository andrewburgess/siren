using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Controller.Controls;
using Controller.Interfaces.Controls;
using Controller.Utilities;
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

		public void SetImage(Model.Image image)
		{
			BitmapImage bitmap;
			try
			{
				bitmap = ImageUtilities.ImageFromBuffer(image.ImageData.ToArray());
			}
			catch (Exception e)
			{
				bitmap = ImageUtilities.ImageFromGDIPlus(image.ImageData.ToArray());
			}
			
			
			((Image) LayoutRoot.Resources["img"]).Source = bitmap;

			LayoutRoot.Background = CreateBrush();
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
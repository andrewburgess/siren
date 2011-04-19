using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Controller;
using Controller.Controls;
using Controller.Interfaces.Controls;
using Controller.Utilities;
using Model;

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
		}

		public void LibraryArtistLoaded(object sender, RoutedEventArgs e)
		{
			Controller.InitializeView();
		}

		private void UserControlMouseUp(object sender, MouseButtonEventArgs e)
		{
			if (e.LeftButton == MouseButtonState.Released)
				SirenController.GetInstance().ShowArtist(Artist);
		}

		#region ILibraryArtistControl Implementation
		public void SetArtistName(string name)
		{
			ArtistName.Text = name;
		}

		public void SetImage(Image image)
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

			ArtistImage.Source = bitmap;

		}
		#endregion
	}
}
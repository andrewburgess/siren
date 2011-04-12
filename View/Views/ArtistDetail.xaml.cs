using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Controller;
using Controller.Controls;
using Controller.Interfaces.Controls;
using Controller.Utilities;
using Model;
using Image = Model.Image;

namespace View.Views
{
	/// <summary>
	/// Interaction logic for ArtistDetail.xaml
	/// </summary>
	public partial class ArtistDetail : IArtistDetailControl
	{
		private ArtistDetailController Controller { get; set; }

		public Artist Artist { get; set; }
		
		public ArtistDetail(Artist artist)
		{
			InitializeComponent();

			Artist = artist;

			Controller = new ArtistDetailController(this);
		}

		private void ArtistDetailLoaded(object sender, RoutedEventArgs e)
		{
			ArtistName.Content = Artist.Name;
			ArtistDescription.Text = Artist.Bio;

			Controller.InitializeView();
		}

		public void SetImages(IQueryable<Image> images)
		{
			foreach (var img in images)
			{
				BitmapImage bitmap;
				try
				{
					bitmap = ImageUtilities.ImageFromBuffer(img.ImageData.ToArray());
				}
				catch (Exception e)
				{
					bitmap = ImageUtilities.ImageFromGDIPlus(img.ImageData.ToArray());
				}

				var i = new System.Windows.Controls.Image();
				i.Source = bitmap;
				ImagePanel.Children.Add(i);
			}
		}

		private void BackButtonClick(object sender, RoutedEventArgs e)
		{
			SirenController.GetInstance().Back();
		}
	}
}
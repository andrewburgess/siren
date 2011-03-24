using System;
using System.Linq;
using System.Windows;
using Controller.Controls;
using Controller.Interfaces.Controls;
using Model;
using Controller.Utilities;

namespace View.Views
{
	/// <summary>
	/// Interaction logic for LibraryLocation.xaml
	/// </summary>
	public partial class LibraryLocation : ILibraryLocationControl
	{
		private LibraryLocationController Controller { get; set; }
		private MediaRepository MediaRepository { get; set; }
		private LibraryWindow Window { get; set; }

		public LibraryLocation(LibraryWindow window, MediaRepository repository)
		{
			InitializeComponent();
			Controller = new LibraryLocationController(this);
			MediaRepository = repository;
			Window = window;

			LocationLabel.Content = repository.Location;

			var size = repository.MediaFiles.Sum(x => x.Size);
			SizeLabel.Content = size.FormatBytes();

			CountLabel.Content = repository.MediaFiles.Count.ToString("N");

			RemoveLocationButton.Click += RemoveLocationButtonClick;
		}

		private void RemoveLocationButtonClick(object sender, RoutedEventArgs e)
		{
			var result =
				MessageBox.Show(
					"Removing this location will delete all files associated with it.\n\nDo you really want to delete this location?",
					"Delete Location", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes)
			{
				Controller.RemoveRepository(MediaRepository);
				Window.RefreshLocations();
			}
		}
	}
}
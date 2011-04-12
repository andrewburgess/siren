using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Controller.Controls;
using Controller.Interfaces.Controls;
using Model;

namespace View.Views
{
	/// <summary>
	/// 	Interaction logic for LibraryControl.xaml
	/// </summary>
	public partial class LibraryControl : ILibraryControl
	{
		private List<LibraryArtist> LibraryArtists { get; set; }

		public LibraryControl()
		{
			InitializeComponent();
			LibraryArtists = new List<LibraryArtist>();
			Controller = new LibraryControlController(this);
		}

		private LibraryControlController Controller { get; set; }

		#region ILibraryControl Members

		public IQueryable<Artist> LibraryItems
		{
			set
			{
				LibraryArtists.Clear();
				LibraryPanel.Children.Clear();
				foreach (var item in value)
				{
					var ctl = new LibraryArtist(item);
					LibraryArtists.Add(ctl);
					LibraryPanel.Children.Add(ctl);
				}
			}
		}

		public void RefreshContent()
		{
			Controller.InitializeView();
		}

		public void UpdateArtist(Artist artist)
		{
			var index = LibraryArtists.FindIndex(x => x.Artist.Id == artist.Id);
			LibraryArtists[index] = new LibraryArtist(artist);
			LibraryPanel.Children.RemoveAt(index);
			LibraryPanel.Children.Insert(index, new LibraryArtist(artist));
		}

		#endregion

		private void LibraryControlLoaded(object sender, RoutedEventArgs e)
		{
			Controller.InitializeView();
		}
	}
}
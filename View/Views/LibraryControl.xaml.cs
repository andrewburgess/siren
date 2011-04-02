using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Controller.Controls;
using Controller.Interfaces.Controls;
using Model;

namespace View.Views
{
	/// <summary>
	/// Interaction logic for LibraryControl.xaml
	/// </summary>
	public partial class LibraryControl : ILibraryControl
	{
		private LibraryControlController Controller { get; set; }

		public LibraryControl()
		{
			InitializeComponent();
			Controller = new LibraryControlController(this);
			Controller.InitializeView();
		}

		public IQueryable<Artist> LibraryItems
		{
			set
			{
				LibraryList.Items.Clear();
				foreach (var item in value)
				{
					var ctl = new LibraryArtist(item);
					LibraryList.Items.Add(ctl);
				}
			}
		}

		private void LibraryListLoaded(object sender, RoutedEventArgs e)
		{
			var view = ((GridView)LibraryList.View);
			if (view.Columns.Count > 0)
			{
				view.Columns[0].Width = LibraryList.ActualWidth - 30;
				foreach (LibraryArtist item in LibraryList.Items)
					item.Width = view.Columns[0].ActualWidth;
			}
		}

		private void LibraryList_LayoutUpdated(object sender, EventArgs e)
		{
			var view = ((GridView)LibraryList.View);
			if (view.Columns.Count > 0)
			{
				view.Columns[0].Width = LibraryList.ActualWidth - 30;
				foreach (LibraryArtist item in LibraryList.Items)
					item.Width = view.Columns[0].ActualWidth;
			}
		}
	}
}
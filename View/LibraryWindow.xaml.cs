using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms;
using Controller;
using Controller.Interfaces;
using Model;
using View.Views;

namespace View
{
	/// <summary>
	/// Interaction logic for LibraryWindow.xaml
	/// </summary>
	public partial class LibraryWindow : ILibraryManagerView
	{
		private LibraryManagerController Controller { get; set; }

		public LibraryWindow()
		{
			InitializeComponent();
			Controller = new LibraryManagerController(this);
		}

		private void AddRepositoryClick(object sender, RoutedEventArgs e)
		{
			var dialog = new FolderBrowserDialog {Description = "Select Folder Containing Music"};
			var result = dialog.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK)
			{
				Controller.AddLocation(dialog.SelectedPath);
			}
		}

		private void CloseDialogClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		public void ClearLocations()
		{
			RepositoryList.Items.Clear();
		}

		public List<MediaRepository> Locations
		{
			set
			{
				foreach (var r in value)
				{
					var control = new LibraryLocation(this, r);
					RepositoryList.Items.Add(control);
				}
			}
		}

		public void RefreshLocations()
		{
			Controller.UpdateLocations();
		}
	}
}
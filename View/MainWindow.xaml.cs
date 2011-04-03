using System;
using System.Windows;
using Controller;
using Controller.Interfaces;
using View.Views;

namespace View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : IMainWindowView
	{
		private SirenController Controller { get; set; }

		private LibraryControl LibraryControl { get; set; }

		public MainWindow()
		{
			InitializeComponent();
			Controller = new SirenController(this);
			Controller.InitializeView();
		}

		private void MenuLibraryManageClick(object sender, RoutedEventArgs e)
		{
			var view = new LibraryWindow();
			view.Show();
		}

		private void MenuLibraryRescanClick(object sender, RoutedEventArgs e)
		{
			var window = new RepositoryScannerWindow();
			window.ShowDialog();
			LibraryControl.RefreshContent();
		}

		public void LoadLibraryView()
		{
			LibraryControl = new LibraryControl();
			ContentPanel.Children.Add(LibraryControl);
		}
	}
}

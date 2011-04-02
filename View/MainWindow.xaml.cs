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
			Controller.RescanLibrary();
		}

		public void LoadLibraryView()
		{
			ContentPanel.Children.Add(new LibraryControl());
		}
	}
}

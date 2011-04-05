using System;
using System.Windows;
using Controller;
using Controller.Interfaces;
using Controller.Interfaces.Controls;
using View.Views;

namespace View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : IMainWindowView
	{
		private SirenController Controller { get; set; }

		public LibraryControl LibControl { get; set; }

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
			LibControl.RefreshContent();
		}

		public void LoadLibraryView()
		{
			LibControl = new LibraryControl();
			ContentPanel.Children.Add(LibControl);
		}

		public ILibraryControl LibraryControl
		{
			get { return LibControl; }
		}

		private void MenuLibrarySettingsClick(object sender, RoutedEventArgs e)
		{
			(new SettingsWindow()).ShowDialog();
		}

		private void MenuQuitClick(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}

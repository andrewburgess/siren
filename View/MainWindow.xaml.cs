using System;
using System.Collections.Generic;
using System.Windows;
using Controller;
using Controller.Interfaces;
using Controller.Interfaces.Controls;
using Model;
using View.Views;

namespace View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : IMainWindowView
	{
		private SirenController Controller { get; set; }
		private Stack<UIElement> Pages { get; set; }

		public LibraryControl LibControl { get; set; }

		public MainWindow()
		{
			InitializeComponent();
			Pages = new Stack<UIElement>();
			Controller = new SirenController(this);
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
			Pages.Push(LibControl);
			ContentPanel.Children.Add(LibControl);
		}

		public ILibraryControl LibraryControl
		{
			get { return LibControl; }
		}

		public void ShowArtistPanel(Artist artist)
		{
			var panel = new ArtistDetail(artist);
			Pages.Push(panel);
			ContentPanel.Children.Clear();
			ContentPanel.Children.Add(panel);
		}

		public void GoBack()
		{
			Pages.Pop();
			ContentPanel.Children.Clear();
			ContentPanel.Children.Add(Pages.Peek());
		}

		private void MenuLibrarySettingsClick(object sender, RoutedEventArgs e)
		{
			(new SettingsWindow()).ShowDialog();
		}

		private void MenuQuitClick(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void MainWindowLoaded(object sender, RoutedEventArgs e)
		{
			Controller.InitializeView();
		}
	}
}

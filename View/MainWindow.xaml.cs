using System.Windows;
using Controller;
using Controller.Interfaces;

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
	}
}

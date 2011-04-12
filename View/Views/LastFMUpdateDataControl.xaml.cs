using System.Windows;
using System.Windows.Controls;
using Controller.Controls;

namespace View.Views
{
	/// <summary>
	/// Interaction logic for LastFMUpdateDataControl.xaml
	/// </summary>
	public partial class LastFMUpdateDataControl : UserControl
	{
		private LastFMUpdateDataController Controller { get; set; }

		public LastFMUpdateDataControl()
		{
			InitializeComponent();

			Controller = new LastFMUpdateDataController();
		}

		private void UpdateAllClick(object sender, RoutedEventArgs e)
		{
			Controller.UpdateAllData();
			MessageBox.Show("Updating Last.FM Data in the background");
		}
	}
}
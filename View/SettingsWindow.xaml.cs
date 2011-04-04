using System.Windows;
using Controller;
using Controller.Interfaces;
using View.Views;

namespace View
{
	/// <summary>
	/// Interaction logic for SettingsWindow.xaml
	/// </summary>
	public partial class SettingsWindow : ISettingsWindow
	{
		private SettingsController Controller { get; set; }

		public SettingsWindow()
		{
			InitializeComponent();
			Controller = new SettingsController(this);
			ContentPanel.Children.Add(new LastFMSettingsControl());
		}

		private void FinishedClick(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}
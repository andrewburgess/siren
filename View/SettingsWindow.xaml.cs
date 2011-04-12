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

		private void LastFMSettings_Selected(object sender, RoutedEventArgs e)
		{
			if (e.Handled == false)
			{
				ContentPanel.Children.Clear();
				ContentPanel.Children.Add(new LastFMSettingsControl());
			}
		}

		private void LastFMUserSettings_Selected(object sender, RoutedEventArgs e)
		{
			ContentPanel.Children.Clear();
			ContentPanel.Children.Add(new LastFMSettingsControl());

			e.Handled = true;
		}

		private void LastFMUpdateData_Selected(object sender, RoutedEventArgs e)
		{
			ContentPanel.Children.Clear();
			ContentPanel.Children.Add(new LastFMUpdateDataControl());

			e.Handled = true;
		}
	}
}
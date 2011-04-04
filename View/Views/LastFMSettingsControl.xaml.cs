using System;
using System.Windows;
using Controller.Controls;
using Controller.Interfaces.Controls;

namespace View.Views
{
	/// <summary>
	/// Interaction logic for LastFMSettingsControl.xaml
	/// </summary>
	public partial class LastFMSettingsControl : ILastFMSettingsControl
	{
		private LastFMSettingsController Controller { get; set; }

		public LastFMSettingsControl()
		{
			InitializeComponent();

			Controller = new LastFMSettingsController(this);
			Controller.InitializeView();
		}

		#region Implementation of ILastFMSettingsControl

		public void SetUsername(string username)
		{
			Username.Text = username;
		}

		public void SetPassword(string password)
		{
			Password.Password = password;
		}

		#endregion

		private void SaveChangesClick(object sender, RoutedEventArgs e)
		{
			Controller.SaveSettings(Username.Text, Password.Password);
			MessageBox.Show("Saved Last.FM Details", "Settings");
		}
	}
}
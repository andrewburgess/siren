using System;
using System.Linq;
using Controller.Interfaces.Controls;
using Model;

namespace Controller.Controls
{
	public class LastFMSettingsController
	{
		private ILastFMSettingsControl View { get; set; }
		private Repository Repository { get; set; }

		public LastFMSettingsController(ILastFMSettingsControl view)
		{
			View = view;
			Repository = DataAccessContext.GetRepository();
		}

		public void InitializeView()
		{
			var username = Repository.Configurations.FirstOrDefault(x => x.Key == Config.LAST_FM_USERNAME);
			var password = Repository.Configurations.FirstOrDefault(x => x.Key == Config.LAST_FM_PASSWORD);

			if (username == null)
			{
				View.SetUsername("");
				View.SetPassword("");
			}
			else
			{
				View.SetUsername(username.Value);
				View.SetPassword(password.Value);
			}
		}

		public void SaveSettings(string username, string password)
		{
			var un = Repository.Configurations.FirstOrDefault(x => x.Key == Config.LAST_FM_USERNAME);
			var pw = Repository.Configurations.FirstOrDefault(x => x.Key == Config.LAST_FM_PASSWORD);

			if (un == null)
			{
				Repository.Configurations.InsertOnSubmit(new Configuration { Key = Config.LAST_FM_USERNAME, Value = username });
				Repository.Configurations.InsertOnSubmit(new Configuration { Key = Config.LAST_FM_PASSWORD, Value = password });
			}
			else
			{
				un.Value = username;
				pw.Value = password;
			}

			Repository.SubmitChanges();
		}
	}
}
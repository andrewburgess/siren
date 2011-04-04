using Controller.Interfaces;
using Model;

namespace Controller
{
	public class SettingsController
	{
		private ISettingsWindow View { get; set; }
		private Repository Repository { get; set; }

		public SettingsController(ISettingsWindow view)
		{
			View = view;
			Repository = DataAccessContext.GetRepository();
		}
	}
}
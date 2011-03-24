using Controller.Interfaces;
using Model;

namespace Controller
{
	public class SirenController
	{
		private IMainWindowView View { get; set; }
		private Repository Repository { get; set; }

		public SirenController(IMainWindowView view)
		{
			View = view;
			Repository = DataAccessContext.GetRepository();
		}
	}
}

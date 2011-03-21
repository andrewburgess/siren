using Controller.Interfaces;

namespace Controller
{
	public class SirenController
	{
		private IMainWindowView View { get; set; }

		public SirenController(IMainWindowView view)
		{
			View = view;
		}
	}
}

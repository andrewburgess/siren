using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Controller.Interfaces;
using Controller.Interfaces.Controls;
using Model;

namespace Controller
{

	public class SirenController
	{
		private IMainWindowView View { get; set; }
		private ILibraryControl LibraryControl { get; set; }
		private Repository Repository { get; set; }

		private LastFMUpdater LastFMUpdater { get; set; }
		private bool Running { get; set; }

		public SirenController(IMainWindowView view)
		{
			View = view;
			
			Repository = DataAccessContext.GetRepository();
			Running = true;
		}

		public void InitializeView()
		{
			View.LoadLibraryView();

			LibraryControl = View.LibraryControl;
			LastFMUpdater = new LastFMUpdater();
			LastFMUpdater.ProgressChanged += LastFMUpdated;
			LastFMUpdater.RunWorkerAsync();
		}

		private void LastFMUpdated(object sender, ProgressChangedEventArgs e)
		{
			LibraryControl.RefreshContent();
		}
	}
}

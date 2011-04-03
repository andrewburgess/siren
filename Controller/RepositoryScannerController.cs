using System;
using System.ComponentModel;
using Controller.Interfaces;

namespace Controller
{
	public class RepositoryScannerController
	{
		private RepositoryScanner Scanner { get; set; }
		private IRepositoryScannerWindow View { get; set; }
		
		public RepositoryScannerController(IRepositoryScannerWindow view)
		{
			View = view;

			Scanner = new RepositoryScanner();
			Scanner.ProgressChanged += ScannerProgressChanged;
			Scanner.RunWorkerCompleted += ScannerCompleted;
			Scanner.RunWorkerAsync();
		}

		private void ScannerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			View.Close();
		}

		private void ScannerProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			var state = (RepositoryScannerState)e.UserState;
			View.SetCurrentRepository(state.CurrentRepository);
			View.SetCurrentPath(state.CurrentPath);
			View.SetProgress(e.ProgressPercentage);
		}
	}
}
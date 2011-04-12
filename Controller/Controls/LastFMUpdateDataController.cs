namespace Controller.Controls
{
	public class LastFMUpdateDataController
	{
		public void UpdateAllData()
		{
			var lastfmUpdater = new LastFMUpdater(LastFMUpdates.All);
			lastfmUpdater.ProgressChanged += SirenController.GetInstance().LastFMUpdate;
			lastfmUpdater.RunWorkerAsync();
		}
	}
}
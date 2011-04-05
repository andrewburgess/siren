using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LastFM;
using Model;

namespace Controller
{

	public class LastFMUpdater : BackgroundWorker
	{
		private Repository Repository { get; set; }
		private bool Working { get; set; }

		public LastFMUpdater()
		{
			Repository = DataAccessContext.GetRepository();
			WorkerReportsProgress = true;
			WorkerSupportsCancellation = false;
			DoWork += Start;
			Working = false;
		}

		private void Start(object sender, DoWorkEventArgs args)
		{
			while (true)
			{
				if (!Working)
				{
					Working = true;
					var artistUpdates =
						Repository.Artists.Where(x => x.LastUpdate.HasValue == false || x.LastUpdate.Value < DateTime.Now.AddSeconds(-1));

					var factory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);
					var tasks = new List<Task<ArtistInfo>>();
					foreach (var a in artistUpdates)
					{
						var a1 = a;
						var task = factory.StartNew(() => UpdateArtist(a1));
						tasks.Add(task);
					}
					if (tasks.Count > 0)
						factory.ContinueWhenAll(tasks.ToArray(), CommitArtistUpdates);
				}

				Thread.Sleep(1000);
			}
		}

		private static ArtistInfo UpdateArtist(Artist artist)
		{
			var data = API.Artist.GetInfo(artist.Id, artist.Name);
			return data;
		}

		private void CommitArtistUpdates(Task<ArtistInfo>[] tasks)
		{
			foreach (var task in tasks)
			{
				var info = task.Result;
				var artist = Repository.Artists.First(x => x.Id == info.Id);
				artist.Bio = info.Bio;
				artist.LastFMListeners = info.Listeners;
				artist.LastFMPlays = info.Plays;
				artist.Summary = info.Summary;
				if (artist.MBID.HasValue == false)
					artist.MBID = info.MBID;
				artist.LastUpdate = DateTime.Now;
				Repository.SubmitChanges();
			}

			ReportProgress(100);

			Wait();
		}

		private void Wait()
		{
			Thread.Sleep(600000);
			Working = false;
		}
	}
}

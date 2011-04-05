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
		private bool Working { get; set; }

		public LastFMUpdater()
		{
			WorkerReportsProgress = true;
			WorkerSupportsCancellation = false;
			DoWork += Start;
		}

		private void Start(object sender, DoWorkEventArgs args)
		{
			while (true)
			{

				var factory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);
				factory.StartNew(WorkOnArtists);
				factory.StartNew(WorkOnUserinfo);

				Thread.Sleep(14400000);
			}
		}

		private void WorkOnUserinfo()
		{
			string username;
			RecentTracks initialPage;
			int count;
			using (var repository = DataAccessContext.GetRepository())
			{
				var configValue = repository.Configurations.FirstOrDefault(x => x.Key == Config.LAST_FM_USERNAME);

				if (configValue == null)
					return;

				username = configValue.Value;

				initialPage = API.User.GetRecentTracks(username, 1, 10);
				count = 0;
				foreach (var t in initialPage.TrackPlays)
				{
					var t1 = t;
					if (repository.Plays.Count(x => x.DatePlayed == t1.DatePlayed) > 0)
					{
						count += 1;
					}
				}
			}

			if (count >= 8)
			{
				//Probably already downloaded track plays from Last.FM, ignore for now
				return;
			}

			initialPage = API.User.GetRecentTracks(username, 1, 200);
			foreach (var track in initialPage.TrackPlays)
			{
				AddTrackPlay(track);
			}
			Parallel.For(2, initialPage.TotalPages, new ParallelOptions {MaxDegreeOfParallelism = 10}, delegate(int index)
			                                        	{
			                                        		var page = API.User.GetRecentTracks(username, index, 200);
															foreach (var track in page.TrackPlays)
															{
																AddTrackPlay(track);
															}
			                                        	});
			ReportProgress(100);
		}

		private static void AddTrackPlay(TrackPlay track)
		{
			using (var repo = DataAccessContext.GetRepository())
			{
				if (repo.Plays.Count(x => x.DatePlayed == track.DatePlayed) > 0)
					return;
				var play = new Play {Id = Guid.NewGuid()};

				var t =
					repo.Tracks.Where(x => x.Name.ToLower() == track.TrackName.ToLower()).Where(
						x => x.Artist.Name.ToLower() == track.Artist.ToLower()).FirstOrDefault();
				if (t == null)
					return;

				play.TrackId = t.Id;
				play.DatePlayed = track.DatePlayed;
				play.Percentage = 100;
				repo.Plays.InsertOnSubmit(play);
				repo.SubmitChanges();
			}
		}

		private void WorkOnArtists()
		{
			using (var repository = DataAccessContext.GetRepository())
			{
				var artistUpdates = repository.Artists.Where(x => x.LastUpdate.HasValue == false || x.LastUpdate.Value < DateTime.Now.AddSeconds(-1));

				var lockObj = new object();
				var info = new List<ArtistInfo>();
				var result = Parallel.ForEach(artistUpdates, new ParallelOptions {MaxDegreeOfParallelism = 4},
				                              delegate(Artist artist)
				                              	{
				                              		var update = UpdateArtist(artist);
				                              		lock (lockObj)
				                              			info.Add(update);
				                              	});
				CommitArtistUpdates(info);
			}
		}

		private static ArtistInfo UpdateArtist(Artist artist)
		{
			var data = API.Artist.GetInfo(artist.Id, artist.Name);
			return data;
		}

		private void CommitArtistUpdates(IEnumerable<ArtistInfo> updates)
		{
			using (var repo = DataAccessContext.GetRepository())
			{
				foreach (var info in updates)
				{
					var artist = repo.Artists.First(x => x.Id == info.Id);
					artist.Bio = info.Bio;
					artist.LastFMListeners = info.Listeners;
					artist.LastFMPlays = info.Plays;
					artist.Summary = info.Summary;
					if (artist.MBID.HasValue == false)
						artist.MBID = info.MBID;
					artist.LastUpdate = DateTime.Now;
					repo.SubmitChanges();
				}
			}

			ReportProgress(100);
		}
	}
}

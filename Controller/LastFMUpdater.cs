using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Controller.Utilities;
using LastFM;
using Model;

namespace Controller
{
	public enum LastFMUpdates
	{
		All,
		UserPlays,
		ArtistInfo,
		ArtistImages
	}

	public class LastFMUpdater : BackgroundWorker
	{
		public LastFMUpdater(LastFMUpdates update)
		{
			WorkerReportsProgress = true;
			WorkerSupportsCancellation = false;
			switch (update)
			{
				case LastFMUpdates.All:
					DoWork += StartAll;
					break;
				case LastFMUpdates.UserPlays:
					DoWork += UpdateUserPlays;
					break;
				case LastFMUpdates.ArtistInfo:
					DoWork += UpdateArtistInfo;
					break;
				case LastFMUpdates.ArtistImages:
					DoWork += UpdateArtistImages;
					break;
			}
		}

		private void StartAll(object sender, DoWorkEventArgs args)
		{
			var finished = false;
			var tasks = new List<Task>();
			var factory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);
			tasks.Add(factory.StartNew(UpdateArtistInfo));
			tasks.Add(factory.StartNew(UpdateUserPlays));
			tasks.Add(factory.StartNew(UpdateArtistImages));

			factory.ContinueWhenAll(tasks.ToArray(), delegate(Task[] t) { finished = true; });

			while (!finished)
			{
				Thread.Sleep(2000);
			}
		}


		private void UpdateUserPlays()
		{
			UpdateUserPlays(null, null);
		}

		private void UpdateUserPlays(object sender, DoWorkEventArgs args)
		{
			string username;
			var from = DateTime.MinValue;
			using (var repository = DataAccessContext.GetRepository())
			{
				var configValue = repository.Configurations.FirstOrDefault(x => x.Key == Config.LAST_FM_USERNAME);

				if (configValue == null)
					return;

				username = configValue.Value;

				configValue = repository.Configurations.FirstOrDefault(x => x.Key == Config.LAST_FM_LAST_PLAYCOUNT_UPDATE);

				if (configValue != null)
				{
					from = DateTime.Parse(configValue.Value);
					configValue.Value = DateTime.Now.ToUniversalTime().ToString();
				}
				else
				{
					var c = new Configuration { Key = Config.LAST_FM_LAST_PLAYCOUNT_UPDATE, Value = DateTime.Now.ToUniversalTime().ToString() };
					repository.Configurations.InsertOnSubmit(c);
				}

				repository.SubmitChanges();
			}

			var initialPage = API.User.GetRecentTracks(username, 1, 200, from);
			foreach (var track in initialPage.TrackPlays)
			{
				AddTrackPlay(track);
			}
			Parallel.For(2, initialPage.TotalPages,
						 new ParallelOptions { MaxDegreeOfParallelism = 10 }, delegate(int index)
																				{
																					var page = API.User.GetRecentTracks(username, index, 200, from);
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
				var play = new Play { Id = Guid.NewGuid() };

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

		private void UpdateArtistInfo()
		{
			UpdateArtistInfo(null, null);
		}

		private void UpdateArtistInfo(object sender, DoWorkEventArgs args)
		{
			using (var repository = DataAccessContext.GetRepository())
			{
				var artistUpdates =
					repository.Artists.Where(x => x.LastUpdate.HasValue == false || x.LastUpdate.Value < DateTime.Now.AddMonths(-1));

				var lockObj = new object();
				var info = new List<ArtistInfo>();
				Parallel.ForEach(artistUpdates, new ParallelOptions { MaxDegreeOfParallelism = 4 },
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
					var info1 = info;
					var artist = repo.Artists.First(x => x.Id == info1.Id);
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

			ReportProgress(100, null);
		}


		private void UpdateArtistImages()
		{
			UpdateArtistImages(null, null);
		}

		private void UpdateArtistImages(object sender, DoWorkEventArgs args)
		{
			var toUpdate = new List<Artist>();
			using (var repo = DataAccessContext.GetRepository())
			{
				var artists = repo.Artists.Select(x => x);

				toUpdate.AddRange(artists.Where(a => repo.Images.Count(x => x.LinkedId == a.Id) == 0));
			}

			Parallel.ForEach(toUpdate, new ParallelOptions {MaxDegreeOfParallelism = 4},
			                 delegate(Artist a)
			                 	{
			                 		var image = API.Artist.GetImages(a.Name, 10);
			                 		foreach (var i in image.ImageUrls)
			                 		{
			                 			var client = new WebClient();
			                 			var bytes = client.DownloadData(i.Value);
			                 			Binary imgData;
			                 			BitmapImage bitmap;
			                 			int height;
			                 			int width;
			                 			try
			                 			{
			                 				bitmap = ImageUtilities.ImageFromBuffer(bytes);
			                 				height = (int) bitmap.Height;
			                 				width = (int) bitmap.Width;
			                 				imgData = new Binary(bytes);
			                 			}
			                 			catch (Exception e)
			                 			{
			                 				try
			                 				{
			                 					bitmap = ImageUtilities.ImageFromGDIPlus(bytes);
			                 					height = (int) bitmap.Height;
			                 					width = (int) bitmap.Width;

			                 					var b = ImageUtilities.BufferFromImage(bitmap);

			                 					//var fs = File.OpenRead(@"C:\Temp\" + guid + ".bmp");
			                 					//var b = new byte[fs.Length];
			                 					//fs.Read(b, 0, (int) fs.Length);
			                 					//fs.Close();

			                 					imgData = new Binary(b);
			                 				}
			                 				catch (Exception e1)
			                 				{
			                 					return;
			                 				}
			                 			}
			                 			using (var repo = DataAccessContext.GetRepository())
			                 			{

			                 				var img = new Image
			                 				          	{
			                 				          		Id = Guid.NewGuid(),
			                 				          		Size = (int) i.Key,
			                 				          		Height = height,
			                 				          		Width = width,
			                 				          		ImageData = imgData,
			                 				          		Url = i.Value,
			                 				          		LinkedId = a.Id
			                 				          	};

			                 				repo.Images.InsertOnSubmit(img);
			                 				repo.SubmitChanges();
			                 			}
			                 		}

			                 		ReportProgress(100, a);
			                 	});
		}

	}
}
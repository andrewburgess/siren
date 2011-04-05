using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Controller.Utilities;
using LastFM;
using Model;
using Image = Model.Image;

namespace Controller
{
	public class LastFMUpdater : BackgroundWorker
	{
		public LastFMUpdater()
		{
			WorkerReportsProgress = true;
			WorkerSupportsCancellation = false;
			DoWork += Start;
		}

		private bool Working { get; set; }

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
			using (Repository repository = DataAccessContext.GetRepository())
			{
				Configuration configValue = repository.Configurations.FirstOrDefault(x => x.Key == Config.LAST_FM_USERNAME);

				if (configValue == null)
					return;

				username = configValue.Value;

				initialPage = API.User.GetRecentTracks(username, 1, 10);
				count = 0;
				foreach (TrackPlay t in initialPage.TrackPlays)
				{
					TrackPlay t1 = t;
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
			foreach (TrackPlay track in initialPage.TrackPlays)
			{
				AddTrackPlay(track);
			}
			Parallel.For(2, initialPage.TotalPages, new ParallelOptions {MaxDegreeOfParallelism = 10}, delegate(int index)
			                                                                                           	{
			                                                                                           		RecentTracks page =
			                                                                                           			API.User.
			                                                                                           				GetRecentTracks(
			                                                                                           					username, index, 200);
			                                                                                           		foreach (
			                                                                                           			TrackPlay track in
			                                                                                           				page.TrackPlays)
			                                                                                           		{
			                                                                                           			AddTrackPlay(track);
			                                                                                           		}
			                                                                                           	});
			ReportProgress(100);
		}

		private static void AddTrackPlay(TrackPlay track)
		{
			using (Repository repo = DataAccessContext.GetRepository())
			{
				if (repo.Plays.Count(x => x.DatePlayed == track.DatePlayed) > 0)
					return;
				var play = new Play {Id = Guid.NewGuid()};

				Track t =
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
			using (Repository repository = DataAccessContext.GetRepository())
			{
				IQueryable<Artist> artistUpdates =
					repository.Artists.Where(x => x.LastUpdate.HasValue == false || x.LastUpdate.Value < DateTime.Now.AddMonths(-1));

				var lockObj = new object();
				var info = new List<ArtistInfo>();
				ParallelLoopResult result = Parallel.ForEach(artistUpdates, new ParallelOptions {MaxDegreeOfParallelism = 4},
				                                             delegate(Artist artist)
				                                             	{
				                                             		ArtistInfo update = UpdateArtist(artist);
				                                             		lock (lockObj)
				                                             			info.Add(update);
				                                             	});
				CommitArtistUpdates(info);
			}
		}

		private static ArtistInfo UpdateArtist(Artist artist)
		{
			ArtistInfo data = API.Artist.GetInfo(artist.Id, artist.Name);
			return data;
		}

		private void CommitArtistUpdates(IEnumerable<ArtistInfo> updates)
		{
			using (Repository repo = DataAccessContext.GetRepository())
			{
				foreach (ArtistInfo info in updates)
				{
					ArtistInfo info1 = info;
					Artist artist = repo.Artists.First(x => x.Id == info1.Id);
					artist.Bio = info.Bio;
					artist.LastFMListeners = info.Listeners;
					artist.LastFMPlays = info.Plays;
					artist.Summary = info.Summary;
					if (artist.MBID.HasValue == false)
						artist.MBID = info.MBID;
					artist.LastUpdate = DateTime.Now;

					repo.SubmitChanges();

					UpdateArtistImages(artist.Id, info.Images);
				}
			}

			ReportProgress(100);
		}

		private static void UpdateArtistImages(Guid id, IEnumerable<KeyValuePair<string, string>> images)
		{
			Parallel.ForEach(images, new ParallelOptions {MaxDegreeOfParallelism = 1},
			                 delegate(KeyValuePair<string, string> image)
			                 	{
			                 		using (Repository repo = DataAccessContext.GetRepository())
			                 		{
			                 			if (repo.Images.Count(x => x.Url == image.Value) > 0)
			                 				return;

			                 			var client = new WebClient();
			                 			byte[] bytes = client.DownloadData(image.Value);
			                 			Binary imgData;
			                 			if (repo.Images.Count(x => x.Url == image.Value) == 0)
			                 			{
			                 				ImageSize size = ImageSize.Small;
			                 				switch (image.Key)
			                 				{
			                 					case "original":
			                 						size = ImageSize.Original;
			                 						break;
			                 					case "large":
			                 						size = ImageSize.Large;
			                 						break;
			                 					case "largesquare":
			                 						size = ImageSize.LargeSquare;
			                 						break;
			                 					case "medium":
			                 						size = ImageSize.Medium;
			                 						break;
			                 					case "small":
			                 						size = ImageSize.Small;
			                 						break;
			                 					case "extralarge":
			                 						size = ImageSize.ExtraLarge;
			                 						break;
			                 					case "mega":
			                 						size = ImageSize.Mega;
			                 						break;
			                 				}

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

			                 				var img = new Image
			                 				          	{
			                 				          		Id = Guid.NewGuid(),
			                 				          		Size = (int) size,
			                 				          		Height = height,
			                 				          		Width = width,
			                 				          		ImageData = imgData,
			                 				          		Url = image.Value,
			                 				          		LinkedId = id
			                 				          	};

			                 				repo.Images.InsertOnSubmit(img);
			                 				repo.SubmitChanges();
			                 			}
			                 		}
			                 	});
		}
	}
}
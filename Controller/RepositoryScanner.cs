using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Controller.Utilities;
using LastFM;
using Model;
using TagLib;
using Image = Model.Image;
using Tag = TagLib.Tag;

namespace Controller
{
	public class RepositoryScannerState
	{
		public string CurrentRepository { get; set; }
		public string CurrentPath { get; set; }
	}

	public class RepositoryScanner : BackgroundWorker
	{
		private static readonly string[] FileExtensions = new[] {".mp3"};

		private readonly object artistLock = new object();
		private readonly object albumLock = new object();

		public enum States
		{
			ScanningDirectories,
			ProcessingFiles
		}

		private Repository Repository { get; set; }

		public RepositoryScanner()
		{
			Repository = DataAccessContext.GetRepository();

			WorkerReportsProgress = true;
			WorkerSupportsCancellation = true;
			DoWork += Scan;
		}

		private void Scan(object sender, DoWorkEventArgs args)
		{
			
			var repositories = Repository.MediaRepositories.Select(x => x);
			var repositoryCount = 0;
			var totalRepositories = repositories.Count();
			foreach (var repository in repositories)
			{
				var paths = new List<string>();
				var directory = new DirectoryInfo(repository.Location);
				paths.AddRange(FindFiles(directory));
				paths.AddRange(ScanSubDirectories(directory));
				repository.LastScanned = DateTime.Now;

				var pathCount = 0;
				Parallel.ForEach(paths, new ParallelOptions {MaxDegreeOfParallelism = 15}, delegate(string path)
				                        	{
												var repo = DataAccessContext.GetRepository();
				                        		pathCount += 1;
				                        		ReportProgress(
				                        			CalculatePercentage(pathCount, paths.Count, repositoryCount, totalRepositories),
				                        			new RepositoryScannerState {CurrentPath = path, CurrentRepository = repository.Location});

												var mediaFile = repo.MediaFiles.FirstOrDefault(x => x.FullPath == path);
				                        		Track track;
				                        		if (mediaFile != null)
				                        		{
				                        			if (mediaFile.LastModified <
				                        			    (new FileInfo(mediaFile.FullPath)).LastWriteTime.AddMinutes(-1))
				                        			{
														track = repo.Tracks.First(x => x.Id == mediaFile.TrackId);
				                        				//Update Track metadata
				                        				ProcessTrack(path, ref track, ref repo);
														repo.SubmitChanges();
				                        			}
				                        		}
				                        		else
				                        		{
				                        			track = new Track {Id = Guid.NewGuid()};
				                        			//Update Track metadata
				                        			ProcessTrack(path, ref track, ref repo);

													repo.Tracks.InsertOnSubmit(track);
													repo.SubmitChanges();

				                        			var file = new FileInfo(path);
				                        			mediaFile = new MediaFile
				                        			            	{
				                        			            		Id = Guid.NewGuid(),
				                        			            		RepositoryId = repository.Id,
				                        			            		FullPath = path,
				                        			            		DateAdded = DateTime.Now,
				                        			            		LastModified = file.LastWriteTime,
				                        			            		FileType = file.Extension,
				                        			            		Size = file.Length,
				                        			            		TrackId = track.Id
				                        			            	};
													repo.MediaFiles.InsertOnSubmit(mediaFile);
													repo.SubmitChanges();
				                        		}
				                        	});
				repositoryCount += 1;
			}
		}

		private static int CalculatePercentage(int pathCount, int totalPaths, int repositoryCount, int totalRepositories)
		{
			//var rP = (float) repositoryCount / totalRepositories;
			//var irP = ((float) repositoryCount + 1) / totalRepositories;
			var pP = (float) pathCount / totalPaths;
			return (int) (pP * 100.0);
		}

		private void ProcessTrack(string path, ref Track track, ref Repository repo)
		{
			var file = TagLib.File.Create(path);
			track.Name = file.Tag.Title;
			track.TrackNumber = (int) file.Tag.Track;
			track.Duration = (int) file.Properties.Duration.TotalSeconds;
			track.BPM = (int) file.Tag.BeatsPerMinute;
			track.BitRate = file.Properties.AudioBitrate;
			track.Comments = file.Tag.Comment;
			track.Composer = "";
			foreach (var composer in file.Tag.Composers.Take(file.Tag.Composers.Count() - 1))
			{
				track.Composer += composer + "; ";
			}
			track.Composer += file.Tag.Composers.Count() > 0 ? file.Tag.Composers.Last() : "";
			track.DiscNumber = (int) file.Tag.Disc;
			track.SampleRate = file.Properties.AudioSampleRate;
			track.Year = (int) file.Tag.Year;
			track.MBID = string.IsNullOrEmpty(file.Tag.MusicBrainzTrackId) ? (Guid?)null : new Guid(file.Tag.MusicBrainzTrackId);
			track.Genre = file.Tag.FirstGenre;

			var artistId = GetArtist(file.Tag, ref repo);
			track.ArtistId = artistId;
			track.AlbumId = GetAlbum(artistId, file.Tag, ref repo);

			if (file.Tag.Pictures.Count() > 0)
			{
				var pic = file.Tag.Pictures[0];
				InsertAlbumArt(track.AlbumId, pic, ref repo);
			}
		}

		private static void InsertAlbumArt(Guid albumId, IPicture pic, ref Repository repo)
		{
			if (repo.Images.Count(x => x.LinkedId == albumId) > 0)
				return;

			var bitmap = ImageUtilities.ImageFromBuffer(pic.Data.ToArray());

			var img = new Image
			          	{
			          		Height = (int) bitmap.Height,
			          		Id = Guid.NewGuid(),
			          		ImageData = pic.Data.ToArray(),
			          		LinkedId = albumId,
			          		Size = (int) ImageSize.ExtraLarge,
			          		Url = "",
			          		Width = (int) bitmap.Width
			          	};
			repo.Images.InsertOnSubmit(img);
			repo.SubmitChanges();
		}

		private Guid GetAlbum(Guid artistId, Tag tag, ref Repository repo)
		{
			Guid id;
			lock (albumLock)
			{
				var album = repo.Albums.FirstOrDefault(x => x.ArtistId == artistId && x.Name == tag.Album);
				if (album == null)
				{
					album = new Album {Id = Guid.NewGuid()};
					album.MBID = string.IsNullOrEmpty(tag.MusicBrainzReleaseId) ? (Guid?) null : new Guid(tag.MusicBrainzReleaseId);
					album.Name = tag.Album;
					album.ArtistId = artistId;
					repo.Albums.InsertOnSubmit(album);
					repo.SubmitChanges();
				}
				id = album.Id;
			}

			return id;
		}

		private Guid GetArtist(Tag tag, ref Repository repo)
		{
			Guid id;
			lock (artistLock)
			{
				var artist = repo.Artists.FirstOrDefault(x => x.Name == tag.FirstArtist);
				if (artist == null)
				{
					artist = new Artist {Id = Guid.NewGuid()};
					artist.MBID = string.IsNullOrEmpty(tag.MusicBrainzArtistId) ? (Guid?) null : new Guid(tag.MusicBrainzArtistId);
					artist.Name = tag.FirstArtist;
					repo.Artists.InsertOnSubmit(artist);
					repo.SubmitChanges();
				}
				id = artist.Id;
			}

			return id;
		}

		private static IEnumerable<string> FindFiles(DirectoryInfo directory)
		{
			return (from file in directory.EnumerateFiles() where FileExtensions.Contains(file.Extension) select file.FullName).ToList();
		}

		private static IEnumerable<string> ScanSubDirectories(DirectoryInfo rootDirectory)
		{
			var paths = new List<string>();

			foreach (var directory in rootDirectory.EnumerateDirectories())
			{
				paths.AddRange(FindFiles(directory));
				paths.AddRange(ScanSubDirectories(directory));
			}

			return paths;
		}
	}
}
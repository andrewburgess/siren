using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Model;
using TagLib.Id3v2;
using TagLib.Mpeg;
using File = TagLib.Mpeg.File;
using Tag = TagLib.Tag;

namespace Controller
{
	public class RepositoryScanner : BackgroundWorker
	{
		private static readonly string[] FileExtensions = new[] {".mp3"};

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
			foreach (var repository in repositories)
			{
				var paths = new List<string>();
				var directory = new DirectoryInfo(repository.Location);
				paths.AddRange(FindFiles(directory));
				paths.AddRange(ScanSubDirectories(directory));
				repository.LastScanned = DateTime.Now;

				foreach (var path in paths)
				{
					var mediaFile = Repository.MediaFiles.FirstOrDefault(x => x.FullPath == path);
					Track track;
					if (mediaFile != null)
					{
						if (mediaFile.LastModified < (new FileInfo(mediaFile.FullPath)).LastWriteTime.AddMinutes(-1))
						{
							track = Repository.Tracks.First(x => x.Id == mediaFile.TrackId);
							//Update Track metadata
							ProcessTrack(path, ref track);
							Repository.SubmitChanges();
						}
						else
						{
							//File hasn't been modified, skip it
							continue;
						}
					}
					else
					{
						track = new Track { Id = Guid.NewGuid() };
						//Update Track metadata
						ProcessTrack(path, ref track);

						Repository.Tracks.InsertOnSubmit(track);
						Repository.SubmitChanges();

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
						Repository.MediaFiles.InsertOnSubmit(mediaFile);
						Repository.SubmitChanges();					
					}
				}
			}
		}

		private void ProcessTrack(string path, ref Track track)
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

			var artistId = GetArtist(file.Tag);
			track.ArtistId = artistId;
			track.AlbumId = GetAlbum(artistId, file.Tag);
		}

		private Guid GetAlbum(Guid artistId, Tag tag)
		{
			var album = Repository.Albums.FirstOrDefault(x => x.ArtistId == artistId && x.Name == tag.Album);
			if (album == null)
			{
				album = new Album { Id = Guid.NewGuid() };
				album.MBID = string.IsNullOrEmpty(tag.MusicBrainzReleaseId) ? (Guid?)null : new Guid(tag.MusicBrainzReleaseId);
				album.Name = tag.Album;
				album.ArtistId = artistId;
				Repository.Albums.InsertOnSubmit(album);
				Repository.SubmitChanges();
			}

			return album.Id;
		}

		private Guid GetArtist(Tag tag)
		{
			var artist = Repository.Artists.FirstOrDefault(x => x.Name == tag.FirstArtist);
			if (artist == null)
			{
				artist = new Artist {Id = Guid.NewGuid()};
				artist.MBID = string.IsNullOrEmpty(tag.MusicBrainzArtistId) ? (Guid?) null : new Guid(tag.MusicBrainzArtistId);
				artist.Name = tag.FirstArtist;
				Repository.Artists.InsertOnSubmit(artist);
				Repository.SubmitChanges();
			}

			return artist.Id;
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
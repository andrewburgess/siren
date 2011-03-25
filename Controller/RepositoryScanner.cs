using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Model;

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
						if (mediaFile.LastModified < (new FileInfo(mediaFile.FullPath)).LastWriteTime)
						{
							track = Repository.Tracks.First(x => x.Id == mediaFile.TrackId);
						}
						else
						{
							//File hasn't been modified, skip it
							continue;
						}
					}
					else
					{
						mediaFile = new MediaFile();
						mediaFile.RepositoryId = repository.Id;
						track = new Track();

						Repository.Tracks.InsertOnSubmit(track);
						Repository.MediaFiles.InsertOnSubmit(mediaFile);
					}

					//Update Track metadata
					ProcessTrack(path, ref track);
				}

				Repository.SubmitChanges();
			}
		}

		private void ProcessTrack(string path, ref Track track)
		{
			
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
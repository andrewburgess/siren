using System;
using Controller.Interfaces.Controls;
using Model;

namespace Controller.Controls
{
	public class LibraryLocationController
	{
		private ILibraryLocationControl View { get; set; }
		private Repository Repository { get; set; }

		public LibraryLocationController(ILibraryLocationControl view)
		{
			View = view;
			Repository = DataAccessContext.GetRepository();
		}

		public void RemoveRepository(MediaRepository mediaRepository)
		{
			Repository.MediaRepositories.DeleteOnSubmit(mediaRepository);
			Repository.SubmitChanges();
		}
	}
}
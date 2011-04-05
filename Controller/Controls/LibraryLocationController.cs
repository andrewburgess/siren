using System;
using Controller.Interfaces.Controls;
using Model;

namespace Controller.Controls
{
	public class LibraryLocationController
	{
		private ILibraryLocationControl View { get; set; }
		private static Repository Repository { get; set; }

		public LibraryLocationController(ILibraryLocationControl view)
		{
			View = view;

			if (Repository == null)
				Repository = DataAccessContext.GetRepository();
		}

		public void RemoveRepository(MediaRepository mediaRepository)
		{
			Repository.MediaRepositories.DeleteOnSubmit(mediaRepository);
			Repository.SubmitChanges();
		}
	}
}
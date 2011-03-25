using System;
using System.Linq;
using Controller.Interfaces;
using Model;

namespace Controller
{
	public class LibraryManagerController
	{
		private ILibraryManagerView View { get; set; }
		private Repository Repository { get; set; }

		/// <summary>
		/// Creates a new LibraryManagerController object
		/// </summary>
		/// <param name="view">The view that the controller manages</param>
		public LibraryManagerController(ILibraryManagerView view)
		{
			View = view;
			Repository = DataAccessContext.GetRepository();
			UpdateLocations();
		}

		/// <summary>
		/// Adds a new location to the set of repositories
		/// </summary>
		/// <param name="location">The absolute path to the root of the repository location</param>
		public void AddLocation(string location)
		{
			var repo = new MediaRepository {DateAdded = DateTime.Now, Location = location, Id = Guid.NewGuid()};
			Repository.MediaRepositories.InsertOnSubmit(repo);
			Repository.SubmitChanges();
			
			UpdateLocations();
		}

		public void UpdateLocations()
		{
			View.ClearLocations();
			var locations = Repository.MediaRepositories.Select(x => x).ToList();
			View.Locations = locations;
		}
	}
}
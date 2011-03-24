using System.Collections.Generic;
using Model;

namespace Controller.Interfaces
{
	public interface ILibraryManagerView
	{
		void ClearLocations();
		List<MediaRepository> Locations { set; }
	}
}
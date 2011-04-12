using System;
using System.Linq;
using Controller.Controls;
using Model;

namespace Controller.Interfaces.Controls
{
	public interface ILibraryControl
	{
		IQueryable<Artist> LibraryItems { set; }
		void RefreshContent();
		void UpdateArtist(Artist artist);
	}
}
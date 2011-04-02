using System.Linq;
using Model;

namespace Controller.Interfaces.Controls
{
	public interface ILibraryControl
	{
		IQueryable<Artist> LibraryItems { set; }
	}
}
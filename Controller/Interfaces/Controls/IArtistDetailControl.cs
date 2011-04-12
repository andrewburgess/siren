using System.Linq;
using Model;

namespace Controller.Interfaces.Controls
{
	public interface IArtistDetailControl
	{
		Artist Artist { get; }
		void SetImages(IQueryable<Image> images);
	}
}
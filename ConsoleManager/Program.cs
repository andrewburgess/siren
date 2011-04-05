using LastFM;

namespace ConsoleManager
{
	class Program
	{
		static void Main(string[] args)
		{
			var lastfm = API.User.GetRecentTracks("deceptacle", 1, 5);
		}
	}
}

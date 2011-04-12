using System;
using LastFM;

namespace ConsoleManager
{
	class Program
	{
		static void Main(string[] args)
		{
			var lastfm = API.Artist.GetImages("Rammstein");

			Console.ReadLine();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;

namespace ConsoleManager
{
	class Program
	{
		static void Main(string[] args)
		{
			var repo = DataAccessContext.GetRepository();
			foreach (var x in repo.Albums)
			{
				Console.WriteLine(x.Id);
			}
			Console.ReadLine();
		}
	}
}

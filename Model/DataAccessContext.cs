using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
	public static class DataAccessContext
	{
		private const string CONNECTION_STRING = "Data Source=|DataDirectory|\\Repository.sdf";
		private static Repository Repository;

		public static Repository GetRepository()
		{
			return Repository ?? (Repository = new Repository(CONNECTION_STRING));
		}
	}
}

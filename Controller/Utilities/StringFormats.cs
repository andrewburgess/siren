using System;

namespace Controller.Utilities
{
	public static class StringFormats
	{
		public static string FormatBytes(this long bytes)
		{
			const int SCALE = 1024;
			var orders = new[] { "GB", "MB", "KB", "Bytes" };
			var max = (long)Math.Pow(SCALE, orders.Length - 1);

			foreach (var order in orders)
			{
				if (bytes > max)
					return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);

				max /= SCALE;
			}
			return "0 Bytes";
		}
	}
}
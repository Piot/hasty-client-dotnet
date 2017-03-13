using System;
namespace Hasty
{
	public class Timestamp
	{
		public Timestamp()
		{
		}

		public static ulong Now()
		{
			var t = DateTime.UtcNow - new DateTime(1970, 1, 1);

			return (ulong)t.TotalMilliseconds;
		}
	}
}

using System;
namespace Hasty
{
	public class Timestamp
	{
		ulong raw;

		private Timestamp(ulong raw)
		{
			this.raw = raw;
		}

		public static Timestamp Now()
		{
			var t = DateTime.UtcNow - new DateTime(1970, 1, 1);

			return new Timestamp((ulong)t.TotalMilliseconds);
		}

		public static TimestampDelta operator- (Timestamp a, Timestamp b)
		{
			return new TimestampDelta(a.raw - b.raw);
		}

		public static Timestamp FromRaw(ulong raw)
		{
			return new Timestamp(raw);
		}

		public ulong Raw
		{
			get
			{
				return raw;
			}
		}

		public string ToDateString
		{
			get
			{
				var time = TimeSpan.FromMilliseconds(raw);
				var startdate = new DateTime(1970, 1, 1) + time;
				var dateString = startdate.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
				return dateString;
			}
		}

		public override string ToString()
		{
			return string.Format("[Timestamp: {0} Raw {1}]", ToDateString, raw);
		}
	}
}

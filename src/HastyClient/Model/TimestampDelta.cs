using System;
namespace Hasty
{
	public class TimestampDelta
	{
		ulong raw;

		public TimestampDelta(ulong raw)
		{
			this.raw = raw;
		}

		public ulong Raw
		{
			get
			{
				return raw;
			}
		}

		public override string ToString()
		{
			return string.Format("[DeltaTime: {0} ms]", raw);
		}
	}
}

using System;
namespace Hasty.Client.Model
{
	public class PingCommand
	{
		ulong ms;
		public PingCommand(ulong ms)
		{
			this.ms = ms;
		}

		public ulong SentTime
		{
			get
			{
				return ms;
			}
		}
	}
}

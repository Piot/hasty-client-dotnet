namespace Hasty.Client.Model
{
	public class PingCommand
	{
		Timestamp sentTime;

		public PingCommand(Timestamp sentTime)
		{
			this.sentTime = sentTime;
		}

		public Timestamp SentTime
		{
			get
			{
				return sentTime;
			}
		}
	}
}

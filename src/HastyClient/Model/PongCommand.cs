namespace Hasty.Client.Model
{
	public class PongCommand
	{
		Timestamp echoedTime;
		Timestamp remoteTimeSent;

		public PongCommand(Timestamp timeSent, Timestamp echoedTime)
		{
			this.echoedTime = echoedTime;
			remoteTimeSent = timeSent;
		}

		public Timestamp EchoedTime
		{
			get
			{
				return echoedTime;
			}
		}
		public Timestamp SentTime
		{
			get
			{
				return remoteTimeSent;
			}
		}
	}
}

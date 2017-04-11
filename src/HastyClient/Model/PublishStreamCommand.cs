namespace Hasty.Client.Model
{
	public class PublishStreamCommand
	{
		ChannelID channel;
		byte[] payload;

		public PublishStreamCommand(ChannelID channel, byte[] payload)
		{
			this.channel = channel;
			this.payload = payload;
		}

		public ChannelID Channel
		{
			get
			{
				return channel;
			}
		}

		public byte[] Payload
		{
			get
			{
				return payload;
			}
		}

		public override string ToString()
		{
			return string.Format("[PublishStreamCommand {0} {1}]", channel, payload);
		}
	}
}

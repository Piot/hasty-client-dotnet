namespace Hasty.Client.Model
{
	public class StreamDataCommand
	{
		ChannelID channel;
		StreamOffset offset;
		byte[] payload;
		bool isAtEndPosition;

		public StreamDataCommand(ChannelID channel, byte[] payload, bool isAtEndPosition, StreamOffset offset)
		{
			this.channel = channel;
			this.payload = payload;
			this.offset = offset;
			this.isAtEndPosition = isAtEndPosition;
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

		public StreamOffset Offset
		{
			get
			{
				return offset;
			}
		}

		public bool IsAtEndPosition
		{
			get
			{
				return isAtEndPosition;
			}
		}

		public override string ToString()
		{
			return string.Format("[StreamDataCommand {0} {1}]", channel, payload);
		}
	}
}

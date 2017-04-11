using Hasty.Client.Model;

namespace Hasty.Client.Serializer
{
	public class PublishStreamSerializer
	{
		public static void SerializePublishStream(IStreamWriter stream, PublishStreamCommand command)
		{
			ChannelIdSerializer.SerializeChannelId(stream, command.Channel);
			stream.WriteOctets(command.Payload);
		}
	}
}

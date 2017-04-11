namespace Hasty.Client.Serializer
{
	public class ChannelIdSerializer
	{
		public static void SerializeChannelId(IStreamWriter stream, ChannelID channelID)
		{
			stream.WriteUint32(channelID.Raw);
		}
	}
}

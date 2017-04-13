using Hasty.Client.Model;

namespace Hasty.Client.Serializer
{
	public class StreamDataSerializer
	{
		public static void SerializeStreamData(IStreamWriter stream, StreamDataCommand command)
		{
			ChannelIdSerializer.SerializeChannelId(stream, command.Channel);
			StreamOffsetSerializer.SerializeStreamOffset(stream, command.Offset);
			stream.WriteBool(command.IsAtEndPosition);
			stream.WriteLength((ushort) command.Payload.Length);
			stream.WriteOctets(command.Payload);
		}
	}
}

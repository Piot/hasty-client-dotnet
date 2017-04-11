using Hasty.Client.Model;

namespace Hasty.Client.Serializer
{
	public class PongSerializer
	{
		public static void SerializePong(IStreamWriter writer, PongCommand pong)
		{
			writer.WriteUint64(pong.EchoedTime.Raw);
		}
	}
}

using Hasty.Client.Model;

namespace Hasty.Client.Serializer
{
	public class PingSerializer
	{
		public static void SerializePing(IStreamWriter writer, PingCommand ping)
		{
			writer.WriteUint64(ping.SentTime.Raw);
		}
	}
}

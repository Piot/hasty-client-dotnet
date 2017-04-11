using Hasty.Client.Api;
using Hasty.Client.Model;

namespace Hasty.Client.Serializer
{
	public class PingDeserializer
	{
		public static bool DeserializePing(IStreamReader stream, out PingCommand ping)
		{
			Timestamp remoteTime;
			TimestampDeserializer.DeserializeTimestamp(stream, out remoteTime);
			ping = new PingCommand(remoteTime);

			return true;
		}
	}
}

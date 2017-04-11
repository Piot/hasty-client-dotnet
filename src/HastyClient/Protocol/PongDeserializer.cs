using Hasty.Client.Api;
using Hasty.Client.Model;

namespace Hasty.Client.Serializer
{
	public class PongDeserializer
	{
		public static bool DeserializePong(IStreamReader stream, out PongCommand pong)
		{
			Timestamp remoteTime;
			TimestampDeserializer.DeserializeTimestamp(stream, out remoteTime);

			Timestamp echoedTime;
			TimestampDeserializer.DeserializeTimestamp(stream, out echoedTime);

			pong = new PongCommand(remoteTime, echoedTime);

			return true;
		}
	}
}

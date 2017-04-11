using Hasty.Client.Api;

namespace Hasty.Client.Serializer
{
	public class TimestampDeserializer
	{
		public static void DeserializeTimestamp(IStreamReader stream, out Timestamp remoteTime)
		{
			var raw = stream.ReadUint64();
			remoteTime = Timestamp.FromRaw(raw);
		}
	}
}

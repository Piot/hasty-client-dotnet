namespace Hasty.Client.Serializer
{
	public class TimestampSerializer
	{
		public static void SerializeTimestamp(IStreamWriter stream, Timestamp remoteTime)
		{
			stream.WriteUint64(remoteTime.Raw);
		}
	}
}

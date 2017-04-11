namespace Hasty.Client.Serializer
{
	public class StreamOffsetSerializer
	{
		public static void SerializeStreamOffset(IStreamWriter stream, StreamOffset offset)
		{
			stream.WriteUint32(offset.Raw);
		}
	}
}

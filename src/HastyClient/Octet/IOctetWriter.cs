namespace Hasty
{
	public interface IOctetWriter
	{
		void WriteUint8(byte a);

		void WriteUint16(ushort a);

		void WriteUint32(uint a);

		byte[] Close();
	}
}

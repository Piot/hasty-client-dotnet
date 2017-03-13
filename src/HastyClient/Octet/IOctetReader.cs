namespace Hasty
{
	public interface IOctetReader
	{
		ushort ReadUint16();

		uint ReadUint32();

		ulong ReadUint64();

		byte ReadUint8();

		byte[] ReadOctets(int octetCount);

		int RemainingOctetCount
		{
			get;
		}
	}
}

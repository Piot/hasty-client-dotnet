using Hasty.Client.Api;

namespace Hasty.Client
{
	public class StreamReader : IStreamReader
	{
		IOctetReader octetReader;

		public StreamReader(IOctetReader octetReader)
		{
			this.octetReader = octetReader;
		}

		public int RemainingOctetCount
		{
			get
			{
				return octetReader.RemainingOctetCount;
			}
		}

		public ushort ReadLength()
		{
			ushort len = ReadUint8();

			if (len > 127U)
			{
				ushort big = (ushort)(ReadUint8() * 256U);
				len = (ushort)(len + big);
			}
			return len;
		}

		public byte[] ReadOctets(int octetCount)
		{
			return octetReader.ReadOctets(octetCount);
		}

		public string ReadString()
		{
			var length = ReadLength();
			var buf = new byte[length];

			for (var i = 0; i < length; ++i)
			{
				buf[i] = ReadUint8();
			}
			var str = System.Text.Encoding.UTF8.GetString(buf);

			return str;
		}

		public ushort ReadUint16()
		{
			return octetReader.ReadUint16();
		}

		public uint ReadUint32()
		{
			return octetReader.ReadUint32();
		}

		public byte ReadUint8()
		{
			return octetReader.ReadUint8();
		}
	}
}

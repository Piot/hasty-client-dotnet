using System;

namespace Hasty.Client
{
	public class StreamWriter : IStreamWriter
	{
		IOctetWriter octetReader;

		public StreamWriter(IOctetWriter octetReader)
		{
			this.octetReader = octetReader;
		}

		public byte[] Close()
		{
			return octetReader.Close();
		}

		public void WriteLength(ushort len)
		{
			var msb = (byte)(len % 256U);

			if (msb > 127U)
			{
				var lsb = (byte)(len / 256U);
				octetReader.WriteUint8((byte)((lsb & 0x7f) | 0x80));
				octetReader.WriteUint8(msb);
			}
			else
			{
				octetReader.WriteUint8(msb);
			}
		}

		public void WriteString(string str)
		{
			WriteLength((ushort)str.Length);
			var buf = System.Text.Encoding.UTF8.GetBytes(str);

			for (var i = 0; i < buf.Length; ++i)
			{
				WriteUint8(buf[i]);
			}
		}

		public void WriteUint16(ushort data)
		{
			octetReader.WriteUint16(data);
		}

		public void WriteUint32(uint data)
		{
			octetReader.WriteUint32(data);
		}

		public void WriteUint64(ulong data)
		{
			octetReader.WriteUint64(data);
		}

		public void WriteUint8(byte data)
		{
			octetReader.WriteUint8(data);
		}

		public void WriteOctets(byte[] data)
		{
			octetReader.WriteOctets(data);
		}

		public void WriteBool(bool v)
		{
			octetReader.WriteUint8(v ? (byte)0x01 : (byte)0x00);
		}
	}
}

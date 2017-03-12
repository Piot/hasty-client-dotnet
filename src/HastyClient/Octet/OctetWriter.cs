using System;
using System.IO;
using Hasty.Client.Shared;

namespace Hasty.Client.Octet
{
	public class OctetWriter : IOctetWriter
	{
		BinaryWriter writer;
		MemoryStream memoryStream;

		public OctetWriter()
		{
			memoryStream = new MemoryStream();
			writer = new BinaryWriter(memoryStream);
		}

		public void WriteUint16(ushort data)
		{
			Write(EndianConverter.Uint16ToBytes(data));
		}

		public void WriteUint32(uint data)
		{
			Write(EndianConverter.Uint32ToBytes(data));
		}

		public void WriteUint8(byte data)
		{
			writer.Write(data);
		}

		public void Write(byte[] data)
		{
			writer.Write(data);
		}

		public byte[] Close()
		{
			var writeBuf = memoryStream.GetBuffer();
			var octetsWritten = (int) memoryStream.Length;
			var bufferToReturn = new byte[octetsWritten];

			Buffer.BlockCopy(writeBuf, 0, bufferToReturn, 0, octetsWritten);

			return bufferToReturn;
		}
	}
}

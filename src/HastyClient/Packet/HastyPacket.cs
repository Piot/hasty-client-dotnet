using System;
using Hasty.Client.Debug;

namespace Hasty.Client.Packet
{
	public class HastyPacket
	{
		byte[] data;

		public byte[] Octets
		{
			get
			{
				return data;
			}
		}
		public HastyPacket(byte[] data)
		{
			if (data.Length < 1)
			{
				throw new Exception("Packets must be at least one octet");
			}
			this.data = data;
		}

		public byte Command
		{
			get
			{
				return data[0];
			}
		}

		public int Length
		{
			get
			{
				return data.Length;
			}
		}

		public override string ToString()
		{
			var hexString = OctetBufferDebug.OctetsToHex(data);

			return string.Format("[Packet: Command={0:X}, Length={1}, Octets={2}]", Command, Length, hexString);
		}
	}
}

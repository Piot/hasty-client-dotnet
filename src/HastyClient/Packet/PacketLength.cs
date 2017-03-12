using System;

namespace Hasty.Client.Packet
{
	public class PacketLength
	{
		public static uint Convert(byte[] data, int index, out int octetsUsed)
		{
			var octetSize = data.Length - index;

			if (octetSize < 1)
			{
				throw new Exception("Must have at least one octet");
			}

			if (data[index] <= 127)
			{
				octetsUsed = 1;
				return data[index];
			}

			if (octetSize < 2)
			{
				throw new Exception("Must have at least two octets");
			}
			octetsUsed = 2;
			return (uint)(data[index] & 0x7f) * (uint)0x100 + (uint)data[index+1];
		}
	}
}

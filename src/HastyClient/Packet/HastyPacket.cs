using System;

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
			return string.Format("[Packet: Command={0}, Length={1}]", Command, Length);
		}
	}
}

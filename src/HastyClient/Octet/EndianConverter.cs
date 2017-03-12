namespace Hasty.Client.Shared
{
	public static class EndianConverter
	{
		public static byte[] Uint32ToBytes(uint intValue)
		{
			var octets = new byte[4];

			octets[0] = (byte)(intValue >> 24);
			octets[1] = (byte)(intValue >> 16);
			octets[2] = (byte)(intValue >> 8);
			octets[3] = (byte)intValue;

			return octets;
		}

		public static uint BytesToUint32(byte[] octets)
		{
			uint v = 0;

			v = ((uint)octets[0] << 24) + ((uint)octets[1] << 16) + ((uint)octets[2] << 8) + (uint)octets[3];

			return v;
		}

		public static byte[] Uint16ToBytes(ushort intValue)
		{
			var octets = new byte[2];

			octets[0] = (byte)(intValue >> 8);
			octets[1] = (byte)(intValue);

			return octets;
		}

		public static ushort BytesToUint16(byte[] octets)
		{
			ushort v = 0;

			v = (ushort)(octets[0] << 8 + octets[1]);

			return v;
		}
	}
}

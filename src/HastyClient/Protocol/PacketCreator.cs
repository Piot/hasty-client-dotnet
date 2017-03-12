using System;
using Hasty.Client.Packet;

namespace Hasty
{
	public class PacketCreator
	{
		public static HastyPacket Create(byte command, byte[] data)
		{
			var tempBuf = new byte[data.Length + 1];

			tempBuf[0] = command;
			Buffer.BlockCopy(data, 0, tempBuf, 1, data.Length);
			return new HastyPacket(tempBuf);
		}
	}
}

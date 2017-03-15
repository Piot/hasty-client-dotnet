using System;
using Hasty.Client.Api;

namespace Hasty.Client.Packet
{
	public class PacketDecoder
	{
		public PacketDecoder()
		{
		}

		public static HastyPacket Decode(byte[] octets, int octetOffset, int octetLength, out int newOffset, ILog log)
		{
			if (octetOffset + octetLength > octets.Length)
			{
				var e = new Exception("Can not decode past array");
				log.Exception(e);
				throw e;
			}
			var octetSize = octetLength;

			if (octetSize < 2)
			{
				newOffset = 0;
				return null;
			}
			int lengthOctetOffset;
			var octetsToWaitFor = (int)PacketLength.Convert(octets, octetOffset,  out lengthOctetOffset);

			if (octetSize - lengthOctetOffset < octetsToWaitFor)
			{
				log.Debug("Waiting for {0} octets lengthOctets {1} offset:{2} buffSize:{3}", octetsToWaitFor, lengthOctetOffset, octetOffset, octetSize);
				newOffset = 0;
				return null;
			}
			log.Debug("Packet done! {0} octets lengthOctets {1}", octetsToWaitFor, lengthOctetOffset);
			newOffset = octetOffset + octetsToWaitFor + lengthOctetOffset;
			var packetData = new byte[octetsToWaitFor];
			Buffer.BlockCopy(octets, octetOffset + lengthOctetOffset, packetData, 0, octetsToWaitFor);
			return new HastyPacket(packetData);
		}
	}
}

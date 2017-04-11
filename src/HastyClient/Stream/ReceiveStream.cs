using Hasty.Client.Packet;
using Hasty.Client.Shared;
using Hasty.Client.Api;

namespace Hasty
{
	public class ReceiveStream
	{
		const int maxPacketSize = 256 * 1024;
		OctetQueue queue = new OctetQueue(maxPacketSize * 2);
		byte[] targetBuffer = new byte[maxPacketSize];
		IPacketReceiver packetReceiver;
		ILog log;

		public ReceiveStream(IPacketReceiver receiver, ILog log)
		{
			this.log = log.CreateLog(typeof(ReceiveStream));
			packetReceiver = receiver;
		}

		public void Add(byte[] octets)
		{
			// log.Debug("Adding to receive stream:{0:X}", octets);
			queue.Enqueue(octets, 0, octets.Length);
			HastyPacket packet = null;
			do
			{
				var octetCount = queue.Peek(targetBuffer, 0, targetBuffer.Length);

				if (octetCount == 0)
				{
					// log.Debug("octetCount zero");
					return;
				}
				int newOffset;
				packet = PacketDecoder.Decode(targetBuffer, 0, octetCount, out newOffset, log);

				if (packet != null)
				{
					// log.Debug("Got packet!");
					queue.Skip(newOffset);
					packetReceiver.ReceivePacket(packet);
				}
				else
				{
					// log.Debug("No packet so far");
				}
			} while (packet != null);
		}
	}
}

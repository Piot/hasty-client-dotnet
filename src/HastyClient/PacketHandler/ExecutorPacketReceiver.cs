using System;
using Hasty.Client.Api;
using Hasty.Client.Command;
using Hasty.Client.Octet;
using Hasty.Client.Packet;

namespace Hasty.Client.PacketHandler
{
	public class ExecutorPacketReceiver : IPacketReceiver
	{
		ICommandTargetCreator targetCreator;
		ILog log;

		public ExecutorPacketReceiver(ICommandTargetCreator targetCreator, ILog log)
		{
			this.log = log.CreateLog(typeof(ExecutorPacketReceiver));

			this.targetCreator = targetCreator;
		}

		public void ReceivePacket(HastyPacket packet)
		{
			var commandTarget = targetCreator.FindCommandTarget(packet.Command, 1);

			if (commandTarget == null)
			{
				var e = new Exception("Couldn't find it");
				log.Exception(e);
				throw e;
			}
			var octetReader = new OctetReader(packet.Octets, log);
			octetReader.ReadUint8();
			var stream = new StreamReader(octetReader);

			commandTarget.Execute(stream);
		}
	}
}

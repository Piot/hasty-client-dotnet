using System;
using System.Collections.Generic;
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
		List<HastyPacket> packetsToProcess = new List<HastyPacket>();

		public ExecutorPacketReceiver(ICommandTargetCreator targetCreator, ILog log)
		{
			this.log = log.CreateLog(typeof(ExecutorPacketReceiver));

			this.targetCreator = targetCreator;
		}

		public void ReceivePacket(HastyPacket packet)
		{
			lock (packetsToProcess)
			{
				packetsToProcess.Add(packet);
			}
		}

		public void Update()
		{
			ProcessPackets();
		}

		private void ProcessPackets()
		{
			lock (packetsToProcess)
			{
				foreach (var packet in packetsToProcess)
				{
					ProcessPacket(packet);
				}
				packetsToProcess.Clear();
			}
		}

		private void ProcessPacket(HastyPacket packet)
		{
			var commandTarget = targetCreator.FindCommandTarget(packet.Command, 1);

			if (commandTarget == null)
			{
				var e = new Exception(string.Format("Couldn't find command {0}", packet.Command));
				log.Exception(e);
				throw e;
			}
			var octetReader = new OctetReader(packet.Octets, log);
			octetReader.ReadUint8();
			var stream = new StreamReader(octetReader, log);
			commandTarget.Execute(stream);
		}
	}
}

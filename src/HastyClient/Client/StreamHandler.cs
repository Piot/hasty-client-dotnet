using Hasty.Client.Command;
using Hasty.Client.PacketHandler;

namespace Hasty.Client.Api
{
	class StreamHandler
	{
		ExecutorPacketReceiver receiver;
		ReceiveStream receiveStream;

		public StreamHandler(object self, CommandDefinitions definitions, ILog log)
		{
			var target = new CommandExecutor(self, log);

			foreach (var definition in definitions.Definitions)
			{
				target.Define(definition.Key, definition.Value);
			}
			receiver = new ExecutorPacketReceiver(target, log);
			receiveStream = new ReceiveStream(receiver, log);
		}

		internal void Receive(byte[] octets)
		{
			receiveStream.Add(octets);
		}
	}
}

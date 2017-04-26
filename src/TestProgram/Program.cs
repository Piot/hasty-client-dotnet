using System;
using Hasty.Client.Api;

namespace TestProgram
{
	public class Program
	{
		HastyClient client;
		ILog log;
		ushort counter;

		public Program(ILog log)
		{
			this.log = log.CreateLog(typeof(Program));
			var definitions = new CommandDefinitions();
			definitions.AddCommand(1, "settext");
			definitions.AddCommand(0xaa, "setcolor");
			client = new HastyClient(new Uri("tcps://localhost:28888"), "com.hastyd.chat.test", "test", "test", definitions, ".db/", log);
			client.Subscribe(0, this);
			client.Subscribe(1, this);
		}

		public void settext_1(IStreamReader reader)
		{
			var id = reader.ReadUint32();
			var title = reader.ReadString();
			var body = reader.ReadString();

			log.Info("***** SetText {0} {1} {2}", id, title, body);
		}

		public void setcolor_1(IStreamReader reader)
		{
			var color = reader.ReadUint16();

			log.Info("*** Set Color {0}", color);
		}

		internal void SendText(string text)
		{
			var cmd = client.CreateCommand(0xaa);

			counter++;
			cmd.Stream.WriteUint16(counter);
			client.SendCommand(0x01, cmd);
		}
	}
}

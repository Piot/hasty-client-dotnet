using System;
using Hasty.Client.Api;

namespace TestProgram
{
	public class Program
	{
		HastyClient client;
		ILog log;

		public Program(ILog log)
		{
			this.log = log.CreateLog(typeof(Program));
			var definitions = new CommandDefinitions();
			definitions.AddCommand(1, "settext");
			definitions.AddCommand(0xaa, "setcolor");
			client = new HastyClient(new Uri("tcps://localhost:3333"), "com.hastydb.test", definitions, ".db/", log);
			client.Subscribe(0, this);
		}

		public void settext_1(IStreamReader reader)
		{
			var id = reader.ReadUint32();
			var title = reader.ReadString();
			var body = reader.ReadString();

			log.Info("***** SetText {0} {1} {2}", id, title, body);
		}
	}
}

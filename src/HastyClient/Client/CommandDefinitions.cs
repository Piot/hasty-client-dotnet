using System.Collections.Generic;

namespace Hasty.Client.Api
{
	public class CommandDefinitions
	{
		Dictionary<byte, string> cmdToName = new Dictionary<byte, string>();

		public CommandDefinitions()
		{
			AddCommand(Commands.ConnectResult, "connectresult");
		}

		public void AddCommand(byte command, string name)
		{
			cmdToName.Add(command, name);
		}

		public Dictionary<byte, string> Definitions
		{
			get
			{
				return cmdToName;
			}
		}
	}
}

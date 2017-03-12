using System;
using System.Collections.Generic;
using System.Reflection;
using Hasty.Client.Api;

namespace Hasty.Client.Command
{
	public class CommandExecutor : ICommandTargetCreator
	{
		Dictionary<byte, string> cmdToName = new Dictionary<byte, string>();
		object self;
		ILog log;

		public CommandExecutor(object self, ILog log)
		{
			this.log = log.CreateLog(typeof(CommandExecutor));
			this.self = self;
		}

		public void Define(byte command, string name)
		{
			cmdToName.Add(command, name);
		}

		private MethodInfo FindMethod(byte command, byte version)
		{
			Type thisType = self.GetType();
			string name;
			var found = cmdToName.TryGetValue(command, out name);

			if (!found)
			{
				var e = new Exception(string.Format("Couldn't find name for {0}", command));
				log.Exception(e);
				throw e;
			}
			MethodInfo theMethod = thisType.GetMethod(name + "_" + version.ToString());

			return theMethod;
		}

		public ICommandTarget FindCommandTarget(byte command, byte version)
		{
			MethodInfo theMethod = FindMethod(command, version);

			if (theMethod != null)
			{
				return new CommandTarget(self, theMethod);
			}
			else
			{
				log.Warning("Couldn't find method:{0}", command);
				return null;
			}
		}
	}
}

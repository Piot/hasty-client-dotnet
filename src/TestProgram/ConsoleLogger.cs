using System;
using Hasty.Client.Api;

namespace TestProgram
{
	public class ConsoleLogger : ILog
	{
		string prefix;
		public ConsoleLogger(string prefix)
		{
			this.prefix = prefix;
		}

		public void Debug(string message, params object[] objects)
		{
			Log(0, message, objects);
		}

		public void Info(string message, params object[] objects)
		{
			Log(1, message, objects);
		}

		public void Trace(string message, params object[] objects)
		{
			Log(3, message, objects);
		}

		public void Warning(string message, params object[] objects)
		{
			Log(4, message, objects);
		}

		public void Exception(Exception e)
		{
			Log(5, "Exception:{0}", e);
		}

		private void Log(int level, string message, params object[] objects)
		{
			if (level == 1)
			{
				Console.ForegroundColor = ConsoleColor.Cyan;
			}
			Console.WriteLine(prefix + ": " + string.Format(message, objects));
			Console.ForegroundColor = ConsoleColor.White;
		}

		public ILog CreateLog(Type t)
		{
			return new ConsoleLogger(t.Name);
		}
	}
}

using System;
using System.Threading;
using Hasty.Client.Api;

namespace TestProgram
{
	class MainClass
	{
		static Program program;
		static ILog log = new ConsoleLogger("main");

		public static void Main(string[] args)
		{
			program = new Program(log);
			StartLoopForever();
		}

		private static void StartLoopForever()
		{
			do
			{
				while (!Console.KeyAvailable)
				{
					Loop();
				}
			} while (Console.ReadKey(true).Key != ConsoleKey.Escape);
		}

		private static void Loop()
		{
			log.Debug("Looping...");
			Thread.Sleep(1000);
		}
	}
}

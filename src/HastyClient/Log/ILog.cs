using System;

namespace Hasty.Client.Api
{
	public interface ILog
	{
		void Trace(string message, params object[] objects);

		void Debug(string message, params object[] objects);

		void Info(string message, params object[] objects);

		void Warning(string message, params object[] objects);

		void Exception(Exception e);

		ILog CreateLog(Type t);
	}
}

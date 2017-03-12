using System.Reflection;
using Hasty.Client.Api;

namespace Hasty.Client.Command
{
	public class CommandTarget : ICommandTarget
	{
		object self;
		MethodInfo methodInfo;

		public CommandTarget(object self, MethodInfo methodInfo)
		{
			this.methodInfo = methodInfo;
			this.self = self;
		}

		public void Execute(IStreamReader stream)
		{
			methodInfo.Invoke(self, new object[] { stream });
		}
	}
}

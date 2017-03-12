using Hasty.Client.Api;

namespace Hasty.Client.Command
{
	public interface ICommandTarget
	{
		void Execute(IStreamReader reader);
	}
}

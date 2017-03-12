namespace Hasty.Client.Command
{
	public interface ICommandTargetCreator
	{
		ICommandTarget FindCommandTarget(byte command, byte version);
	}
}

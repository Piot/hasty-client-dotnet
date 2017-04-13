namespace Hasty.Client.Api
{
	public class Command
	{
		IStreamWriter writer;
		byte commandId;

		internal Command(IStreamWriter writer, byte commandId)
		{
			this.writer = writer;
			this.commandId = commandId;
		}

		public IStreamWriter Stream
		{
			get
			{
				return writer;
			}
		}

		public byte CommandId
		{
			get
			{
				return commandId;
			}
		}
	}
}

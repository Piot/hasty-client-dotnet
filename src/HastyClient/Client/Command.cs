namespace Hasty.Client.Api
{
	public class Command
	{
		IStreamWriter writer;
		internal Command(IStreamWriter writer, byte commandId)
		{
			this.writer = writer;
			writer.WriteUint8(commandId);
		}

		public IStreamWriter Stream
		{
			get
			{
				return writer;
			}
		}
	}
}

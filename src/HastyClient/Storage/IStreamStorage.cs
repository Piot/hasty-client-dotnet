namespace Hasty.Client.Storage
{
	public interface IStreamStorage
	{
		void Append(uint id, byte[] octets, uint offset, uint octetsToWrite);

		byte[] ReadAll(uint streamId);

		long FileSize(uint streamId);
	}
}

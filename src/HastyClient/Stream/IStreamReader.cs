namespace Hasty.Client.Api
{
	public interface IStreamReader : IOctetReader
	{
		string ReadString();

		ushort ReadLength();
	}
}

namespace Hasty.Client
{
	public interface IStreamWriter : IOctetWriter
	{
		void WriteString(string s);

		void WriteLength(ushort length);
	}
}

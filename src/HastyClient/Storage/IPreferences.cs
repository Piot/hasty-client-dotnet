namespace Hasty.Client.Storage
{
	public interface IPreferences
	{
		void SetUserChannel(string endpoint, string realm, ulong channel);

		bool GetUserChannel(string endpoint, string realm, out ulong channel);
	}
}

namespace Hasty.Client.Model
{
	public class ConnectCommand
	{
		Version protcolVersion;
		string realm;

		public ConnectCommand(Version version, string realm)
		{
			this.protcolVersion = version;
			this.realm = realm;
		}

		public string Realm
		{
			get
			{
				return realm;
			}
		}

		public Version ProtocolVersion
		{
			get
			{
				return protcolVersion;
			}
		}
	}
}

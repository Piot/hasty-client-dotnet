using Hasty.Client.Model;

namespace Hasty.Client.Serializer
{
	public class ConnectSerializer
	{
		public static void SerializeConnect(IStreamWriter writer, ConnectCommand connect)
		{
			VersionSerializer.SerializeVersion(writer, connect.ProtocolVersion);
			var realm = connect.Realm;
			writer.WriteString(realm);
		}
	}
}

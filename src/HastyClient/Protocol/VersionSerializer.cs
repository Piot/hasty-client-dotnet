using Hasty.Client.Model;

namespace Hasty.Client.Serializer
{
	class VersionSerializer
	{
		public static void SerializeVersion(IStreamWriter stream, Version version)
		{
			stream.WriteUint8(version.Major);
			stream.WriteUint8(version.Minor);
			stream.WriteUint8(version.Patch);
		}
	}
}

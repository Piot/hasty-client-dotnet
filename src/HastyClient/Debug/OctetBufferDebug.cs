using System;
using Hasty.Client.Api;

namespace Hasty.Client.Debug
{
	public class OctetBufferDebug
	{
		private static readonly uint[] g_lookupTable = CreateLookupTable();
		const int lookupTableSize = 256;

		private static uint[] CreateLookupTable()
		{
			var result = new uint[lookupTableSize];

			for (int i = 0; i < lookupTableSize; i++)
			{
				string s = i.ToString("X2");
				result[i] = ((uint)s[0]) + ((uint)s[1] << 16);
			}
			return result;
		}

		public static string OctetsToHex(byte[] bytes)
		{
			var octetToHexLookup = g_lookupTable;
			var result = new char[bytes.Length * 3];
			var pos = 0;

			for (int i = 0; i < bytes.Length; i++)
			{
				var val = octetToHexLookup[bytes[i]];
				result[pos++] = (char)val;
				result[pos++] = (char)(val >> 16);
				result[pos++] = ' ';
			}
			return new string(result);
		}

		public static void DebugOctets(byte[] buffer, string description, ILog log)
		{
			var hex = OctetsToHex(buffer);

			log.Debug("{0} {1}", description, hex);
		}
	}
}

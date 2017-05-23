using System.IO;
using Hasty.Client.Api;

namespace Hasty.Client.Storage
{
	public class Preferences : IPreferences
	{
		public struct Data
		{
			public string Endpoint;
			public string Realm;
			public ulong Channel;
		}

		string baseDir;
		ILog log;

		public Preferences(string baseDir, ILog log)
		{
			this.log = log.CreateLog(typeof(Preferences));

			var baseDirectory = Path.GetDirectoryName(baseDir);

			if (!Directory.Exists(baseDirectory))
			{
				Directory.CreateDirectory(baseDirectory);
			}
			this.baseDir = baseDirectory;
			log.Warning("Preferences are stored in:'{0}'", baseDir);
		}

		public static string Serialize(Data data)
		{
			return string.Format("{0};{1};{2};", data.Endpoint, data.Realm, data.Channel);
		}

		public static Data Deserialize(string serialize)
		{
			var parts = serialize.Split(';');
			var channelId = ulong.Parse(parts[2]);

			return new Data
			       {
				       Endpoint = parts[0], Realm = parts[1], Channel = channelId
			       };
		}

		public bool GetUserChannel(string endpoint, string realm, out ulong channelId)
		{
			var s = ReadAll();

			channelId = 0;

			if (s == string.Empty)
			{
				return false;
			}
			var data = Deserialize(s);

			if (endpoint != data.Endpoint || realm != data.Realm)
			{
				return false;
			}

			channelId = data.Channel;

			return true;
		}

		public void SetUserChannel(string endpoint, string realm, ulong channelId)
		{
			var o = new Data
			{
				Endpoint = endpoint, Realm = realm, Channel = channelId
			};

			var s = Serialize(o);

			WriteAll(s);
		}

		private string PreferencePath()
		{
			var path = Path.Combine(baseDir, ".preferences");

			return path;
		}

		private string ReadAll()
		{
			var path = PreferencePath();

			if (!File.Exists(path))
			{
				return string.Empty;
			}

			var octets = File.ReadAllText(path);

			return octets;
		}

		private void WriteAll(string data)
		{
			var path = PreferencePath();

			File.WriteAllText(path, data);
		}
	}
}

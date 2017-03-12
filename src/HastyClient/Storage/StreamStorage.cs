using System;
using System.IO;
using Hasty.Client.Api;

namespace Hasty.Client.Storage
{
	public class StreamStorage : IStreamStorage
	{
		string baseDir;
		ILog log;

		public StreamStorage(string baseDir, ILog log)
		{
			this.log = log.CreateLog(typeof(StreamStorage));

			var baseDirectory = Path.GetDirectoryName(baseDir);

			if (!Directory.Exists(baseDirectory))
			{
				Directory.CreateDirectory(baseDirectory);
			}
			this.baseDir = baseDirectory;
		}

		private string PathFromId(uint id)
		{
			var streamFilename = "" + id;
			var path = Path.Combine(baseDir, streamFilename);

			return path;
		}

		public long FileSize(uint id)
		{
			var path = PathFromId(id);

			if (!File.Exists(path))
			{
				return 0;
			}
			var file = File.OpenRead(path);

			var alreadyWrittenOctetCount = file.Seek(0, SeekOrigin.End);

			file.Close();
			return alreadyWrittenOctetCount;
		}

		public void Append(uint id, byte[] octets, uint startIndexInData, uint octetsToWrite)
		{
			var path = PathFromId(id);
			FileStream file;

			if (!File.Exists(path))
			{
				file = File.Open(path, FileMode.Create);
			}
			else
			{
				file = File.Open(path, FileMode.Append);
			}
			var alreadyWrittenOctetCount = file.Seek(0, SeekOrigin.End);
			log.Debug("Writing to file '{0}' pos:{1} data:{2}", path, alreadyWrittenOctetCount, octets.Length);

			log.Debug("Writing to file '{0}' octets:{1} pos:{2}", path, startIndexInData, alreadyWrittenOctetCount);
			file.Write(octets, (int)startIndexInData, (int)octetsToWrite);
			file.Close();
			file.Dispose();
		}

		public byte[] ReadAll(uint streamId)
		{
			var path = PathFromId(streamId);

			if (!File.Exists(path))
			{
				return new byte[] { };
			}

			var octets = File.ReadAllBytes(path);

			return octets;
		}
	}
}

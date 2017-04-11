using System;
using Hasty.Client.Api;
using Hasty.Client.Shared;

namespace Hasty.Client.Octet
{
	public class OctetReader : IOctetReader
	{
		byte[] octets;
		int pos;
		ILog log;

		public int RemainingOctetCount
		{
			get
			{
				return octets.Length - pos;
			}
		}

		public OctetReader(byte[] data, ILog log)
		{
			this.log = log.CreateLog(typeof(OctetReader));
			octets = data;
			pos = 0;
			// log.Debug("OctetReader:{0}", data.Length);
		}

		public ushort ReadUint16()
		{
			return EndianConverter.BytesToUint16(ReadOctets(2));
		}

		public uint ReadUint32()
		{
			return EndianConverter.BytesToUint32(ReadOctets(4));
		}

		public ulong ReadUint64()
		{
			return EndianConverter.BytesToUint64(ReadOctets(8));
		}

		public byte ReadUint8()
		{
			var v = octets[pos++];

			return v;
		}

		public byte[] ReadOctets(int octetCount)
		{
			if (pos + octetCount > octets.Length)
			{
				var e = new Exception(string.Format("Reading too far {0} {1}", pos, octetCount));
				log.Exception(e);
				throw e;
			}
			var buf = new byte[octetCount];

			Buffer.BlockCopy(octets, pos, buf, 0, octetCount);
			pos += octetCount;

			return buf;
		}
	}
}

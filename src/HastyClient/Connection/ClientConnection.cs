using System;
using System.IO;
using Hasty.Client.Debug;
using Hasty.Client.Shared;
using Hasty.Client.Api;

namespace Hasty.Client.Connection
{
	public class ClientConnection
	{
		Stream stream;
		public delegate void Action();

		public Action OnDisconnected;
		public Action<OctetQueue> OnOctetQueueChanged;

		private byte[] receiveBuffer = new byte[8142];
		const int bufSize = 8192;
		OctetQueue octetQueue = new OctetQueue(bufSize);
		ILog log;

		public ClientConnection(Stream stream, ILog log)
		{
			this.log = log.CreateLog(typeof(ClientConnection));
			this.stream = stream;
		}

		public void Start()
		{
			continueReceiving();
		}

		public void Close()
		{
			if (stream != null)
			{
				stream.Dispose();
				stream = null;
			}
		}

		void continueReceiving()
		{
			stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, new AsyncCallback(ReceiveCallback), null);
		}

		void ReceiveCallback(IAsyncResult ar)
		{
			if (!ar.IsCompleted)
			{
				log.Warning("AR is not completed!");
				return;
			}

			if (stream == null)
			{
				log.Warning("Stream is null");
				return;
			}
			int receivedOctets = -1;
			try
			{
				receivedOctets = stream.EndRead(ar);
			}
			catch (Exception e)
			{
				log.Exception(e);
				throw e;
			}

			if (receivedOctets <= 0)
			{
				log.Warning("Negative octets");
				Disconnected("Receive");
				return;
			}
			LogOctets(receiveBuffer, receivedOctets);
			octetQueue.Enqueue(receiveBuffer, 0, receivedOctets);
			OnOctetQueueChanged(octetQueue);
			continueReceiving();
		}

		public void Write(byte[] data)
		{
			// log.Debug("Write:{0}", OctetBufferDebug.OctetsToHex(data));
			stream.Write(data, 0, data.Length);
			stream.Flush();
		}

		void Disconnected(string reason)
		{
			log.Warning("** Connection is now disconnected! Reason {0} **", reason);
			OnDisconnected();
		}

		void LogOctets(byte[] buf, int receivedOctets)
		{
			var logBuf = new byte[receivedOctets];

			Buffer.BlockCopy(buf, 0, logBuf, 0, receivedOctets);
			log.Debug("Transport Received:'{0}'", OctetBufferDebug.OctetsToHex(logBuf));
		}

		internal void Disconnect(string reason)
		{
			Close();
			Disconnected(reason);
		}
	}
}

using System;
using System.IO;
using System.Timers;
using Hasty.Client.Debug;
using Hasty.Client.Model;
using Hasty.Client.Octet;
using Hasty.Client.Packet;
using Hasty.Client.Serializer;
using Hasty.Client.Shared;
using Hasty.Client.Api;

namespace Hasty.Client.Connection
{
	public class ConnectionMaintainer
	{
		Uri serverUrl;
		Timer timer;
		ClientConnection connection;
		Random random;
		string realm;
		public delegate void Action();

		public Action<HastyPacket> OnPacketRead;
		public Action OnDisconnect;
		public Action OnConnecting;
		public Action OnConnected;
		ILog log;

		public ConnectionMaintainer(Uri serverUrl, string realm, ILog log)
		{
			this.log = log.CreateLog(typeof(ConnectionMaintainer));

			random = new Random();
			this.serverUrl = serverUrl;
			this.realm = realm;
		}

		public void Start()
		{
			Connect();
		}

		private void Connect()
		{
			OnConnecting();
			log.Debug("Try to connect!");
			try
			{
				var stream = Connector.Connect(serverUrl, log);
				SetupStream(stream);
			}
			catch (Exception)
			{
				ConnectAgainLater();
			}
		}

		private void SetupStream(Stream stream)
		{
			log.Debug("We are connected!");
			connection = new ClientConnection(stream, log);
			connection.OnDisconnected += ConnectionDisconnected;
			connection.OnOctetQueueChanged += OctetQueueChanged;
			connection.Start();
			OnConnected();
			WritePacket(ConnectPacket());
		}

		internal void Write(byte[] octets)
		{
			var packet = new HastyPacket(octets);

			WritePacket(packet);
		}

		private HastyPacket ConnectPacket()
		{
			var writer = new OctetWriter();
			var outStream = new StreamWriter(writer);

			var protocolVersion = new Model.Version(0, 0, 1);
			var cmd = new ConnectCommand(protocolVersion, realm);

			ConnectSerializer.SerializeConnect(outStream, cmd);
			var payload = writer.Close();

			var packet = PacketCreator.Create(Commands.Connect, payload);
			return packet;
		}

		private void WritePacket(HastyPacket packet)
		{
			connection.Write(new byte[] { (byte)packet.Length });

			log.Debug("Sending packet:{0} {1}", packet, OctetBufferDebug.OctetsToHex(packet.Octets));
			connection.Write(packet.Octets);
		}

		private void OctetQueueChanged(OctetQueue queue)
		{
			log.Debug("OctetQueue changed {0}", queue);
			var tempBuf = new byte[1024];
			while (true)
			{
				var octetCount = queue.Peek(tempBuf, 0, tempBuf.Length);

				if (octetCount <= 0)
				{
					return;
				}
				int octetsUsed = 0;
				var packet = PacketDecoder.Decode(tempBuf, 0, octetCount, out octetsUsed, log);

				if (packet == null)
				{
					return;
				}
				queue.Skip(octetsUsed);
				try
				{
					OnPacketRead(packet);
				}
				catch (Exception e)
				{
					log.Exception(e);
					throw e;
				}
			}
		}

		private void ConnectionDisconnected()
		{
			log.Warning("Connection is disconnected");
			connection.Close();
			connection = null;
			OnDisconnect();
			ConnectAgainLater();
		}

		private void ConnectAgainLater()
		{
			log.Debug("Connect later...");
			var waitTime = random.Next(800, 2000);
			timer = new Timer() {
				Interval = waitTime
			};
			timer.Elapsed += TimerElapsed;
			timer.Start();
		}

		private void TimerElapsed(object sender, ElapsedEventArgs args)
		{
			timer.Stop();
			timer.Dispose();
			timer = null;

			log.Debug("Timer Elapsed!");
			Connect();
		}
	}
}

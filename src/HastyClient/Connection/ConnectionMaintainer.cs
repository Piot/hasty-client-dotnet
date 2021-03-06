using System;
using System.IO;
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
		Uri mainUrl;
		OneShotTimer connectAgainTimer;
		IntervalTimer pingTimer;
		ClientConnection connection;
		Random random;
		string realm;
		public delegate void Action();

		public Action<HastyPacket> OnPacketRead;
		public Action OnDisconnect;
		public Action OnConnecting;
		public Action OnConnected;
		ILog log;
		Timestamp lastReceivedPacketTime;
		bool shouldRetainConnection = true;

		public ConnectionMaintainer(Uri serverUrl, string realm, ILog log)
		{
			this.log = log.CreateLog(typeof(ConnectionMaintainer));

			random = new Random();
			this.serverUrl = serverUrl;
			mainUrl = serverUrl;
			this.realm = realm;
		}

		public string Realm
		{
			get
			{
				return realm;
			}
		}

		public string Endpoint
		{
			get
			{
				return serverUrl.ToString();
			}
		}

		public void Start()
		{
			Connect();
		}

		public bool IsFocus
		{
			set
			{
				if (value)
				{
					if (!shouldRetainConnection)
					{
						shouldRetainConnection = true;
						Start();
					}
				}
				else
				{
					if (shouldRetainConnection)
					{
						shouldRetainConnection = false;
						Disconnect("Out of focus");
					}
				}
			}
		}

		internal void SwitchToTemporaryHost(Uri url)
		{
			connection.Disconnect("redirected");
			serverUrl = url;
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
			lastReceivedPacketTime = Timestamp.Now();

			connection = new ClientConnection(stream, log);
			connection.OnDisconnected += ConnectionDisconnected;
			connection.OnOctetQueueChanged += OctetQueueChanged;
			connection.Start();
			OnConnected();
			pingTimer = new IntervalTimer(10000);
			pingTimer.OnElapsed += PingUpdate;
			pingTimer.Start();
		}

		void PingUpdate()
		{
			log.Debug("OnPingTime");
			SendPing(Timestamp.Now());
			var timeSinceLastHeardSomething = Timestamp.Now() - lastReceivedPacketTime;

			if (timeSinceLastHeardSomething.Raw > 15000)
			{
				connection.Disconnect("timedout");
			}
		}

		internal void Write(byte[] octets)
		{
			var packet = new HastyPacket(octets);

			WritePacket(packet);
		}

		public void SendPing(Timestamp sentTime)
		{
			WritePacket(PingPacket(sentTime));
		}

		private HastyPacket PingPacket(Timestamp ms)
		{
			var writer = new OctetWriter();
			var outStream = new StreamWriter(writer);
			var cmd = new PingCommand(ms);

			PingSerializer.SerializePing(outStream, cmd);
			var payload = writer.Close();
			var packet = PacketCreator.Create(Commands.Ping, payload);
			return packet;
		}

		private IStreamWriter createStreamWriter()
		{
			var octetWriter = new OctetWriter();
			var streamWriter = new StreamWriter(octetWriter);

			return streamWriter;
		}

		void WritePacket(HastyPacket packet)
		{
			if (connection == null)
			{
				log.Warning("CAN NOT SEND. NO CONNECTION!");
				return;
			}

			// log.Debug("Sending packet:{0} {1}", packet, OctetBufferDebug.OctetsToHex(packet.Octets));

			var stream = createStreamWriter();
			stream.WriteLength((ushort)packet.Length);
			stream.WriteOctets(packet.Octets);
			var octets = stream.Close();
			connection.Write(octets);
		}

		private void OctetQueueChanged(OctetQueue queue)
		{
			// log.Debug("OctetQueue changed {0}", queue);
			lastReceivedPacketTime = Timestamp.Now();
			var tempBuf = new byte[32*1024];
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
			log.Warning("Connection is disconnected. Stopping all timers");

			if (pingTimer != null)
			{
				pingTimer.Stop();
				pingTimer = null;
			}

			if (connection != null)
			{
				connection.Close();
				connection = null;
			}
			OnDisconnect();
			// Go back to main url if we get disconnected
			serverUrl = mainUrl;

			if (shouldRetainConnection)
			{
				ConnectAgainLater();
			}
		}

		private void ConnectAgainLater()
		{
			log.Debug("Connect later...");
			var waitTime = random.Next(800, 2000);

			connectAgainTimer = new OneShotTimer(waitTime);
			connectAgainTimer.OnElapsed += ConnectElapsed;
			connectAgainTimer.Start();
		}

		private void ConnectElapsed()
		{
			connectAgainTimer.Stop();
			connectAgainTimer = null;

			log.Debug("Timer Elapsed!");
			Connect();
		}

		public void Disconnect(string reason)
		{
			log.Debug("Forced disconnect. Reason '{0}'", reason);

			if (connection == null)
			{
				ConnectionDisconnected();
				return;
			}
			connection.Disconnect(reason);
		}
	}
}

using System;
using System.Collections.Generic;
using Hasty.Client.Connection;
using Hasty.Client.Model;
using Hasty.Client.Octet;
using Hasty.Client.Packet;
using Hasty.Client.PacketHandler;
using Hasty.Client.Serializer;
using Hasty.Client.Storage;

namespace Hasty.Client.Api
{
	public class HastyClient
	{
		ConnectionMaintainer connectionMaintainer;

		ConnectionStatus status;
		ConnectionState state;

		IStreamStorage streamStorage;
		IPreferences preferences;
		List<uint> subscribingChannels = new List<uint>();
		Dictionary<uint, StreamHandler> chunkHandlers = new Dictionary<uint, StreamHandler>();
		CommandDefinitions definitions;

		ILog log;
		string username;
		string password;
		string realm;
		object defaultTarget;

		public HastyClient(Uri serverUrl, string realm, string username, string password, CommandDefinitions definitions, string baseDir, object target, ILog log)
		{
			this.username = username;
			this.password = password;
			this.realm = realm;
			this.log = log.CreateLog(typeof(HastyClient));
			var storage = new StreamStorage(baseDir, log);
			preferences = new Preferences(baseDir, log);
			streamStorage = storage;
			this.definitions = definitions;
			defaultTarget = target;

			SetStatus(ConnectionStatus.Idle);
			SetState(ConnectionState.Establishing);
			connectionMaintainer = new ConnectionMaintainer(serverUrl, realm, log);
			connectionMaintainer.OnPacketRead += OnPacketRead;
			connectionMaintainer.OnDisconnect += OnDisconnect;
			connectionMaintainer.OnConnecting += OnConnecting;
			connectionMaintainer.OnConnected += OnMaintainerConnected;

			ulong userAssignedChannelId;

			var foundUserAssignedChannel = preferences.GetUserChannel(serverUrl.ToString(), realm, out userAssignedChannelId);

			if (foundUserAssignedChannel)
			{
				Subscribe((uint)userAssignedChannelId, target);
			}

			connectionMaintainer.Start();
		}

		public void Dispose()
		{
			IsFocus = false;
		}

		public bool IsFocus
		{
			set
			{
				connectionMaintainer.IsFocus = value;
			}
		}
		private ulong ms()
		{
			var t = DateTime.UtcNow - new DateTime(1970, 1, 1);
			return  (ulong)t.TotalMilliseconds;
		}

		public void Update()
		{
			var timestampStart = ms();
			var copy = new Dictionary<uint, StreamHandler>(chunkHandlers);

			foreach (var chunkHandler in copy)
			{
				chunkHandler.Value.Update();
			}

			var totalTime = ms() - timestampStart;
			if (totalTime > 150)
			{
				log.Warning("{0} FINISHED PROCESS", totalTime);
			}
		}

		void OnDisconnect()
		{
			SetStatus(ConnectionStatus.Disconnected);
		}

		void OnConnecting()
		{
			SetStatus(ConnectionStatus.Connecting);
		}

		void OnMaintainerConnected()
		{
			SendPacket(ConnectPacket().Octets);
		}

		void OnConnected()
		{
			SetStatus(ConnectionStatus.Connected);

			Login();
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

		void Login()
		{
			SetState(ConnectionState.LoggingIn);
			SendLogin(username, password);
		}

		void OnLoggedIn(IStreamReader stream)
		{
			var userChannelID = stream.ReadUint32();

			if (state == ConnectionState.LoggedIn)
			{
				log.Debug("OnLoggedIn - ignoring...");
				return;
			}
			log.Debug("OnLoggedIn");
			Subscribe(userChannelID, defaultTarget);
			preferences.SetUserChannel(connectionMaintainer.Endpoint, connectionMaintainer.Realm, userChannelID);

			if (subscribingChannels.Count != 0)
			{
				var copy = new List<uint>(subscribingChannels);
				foreach (var subscribingChannel in copy)
				{
					var offset = (uint) streamStorage.FileSize(subscribingChannel);
					SendSubscribe(subscribingChannel, offset);
				}
			}
			SetState(ConnectionState.LoggedIn);
		}

		IStreamWriter CreateStreamInternal()
		{
			var octetWriter = new OctetWriter();
			var streamWriter = new StreamWriter(octetWriter);

			return streamWriter;
		}

		IStreamWriter CreateStream(byte command)
		{
			var streamWriter = CreateStreamInternal();

			streamWriter.WriteUint8(command);
			return streamWriter;
		}

		public void Subscribe(uint streamId, object target)
		{
			StreamHandler existingHandler;
			var alreadyInThere = chunkHandlers.TryGetValue(streamId, out existingHandler);

			if (alreadyInThere)
			{
				// log.Warning(string.Format("You are already subscribed to {0}. Ignoring for now.", streamId));
				return;
			}

			var streamHandler = new StreamHandler(target, definitions, log);
			chunkHandlers.Add(streamId, streamHandler);
			subscribingChannels.Add(streamId);
			var data = streamStorage.ReadAll(streamId);
			InternalOnStreamData(streamId, data, 0, (uint)data.Length);

			if (IsConnected)
			{
				SendSubscribe(streamId, (uint)data.Length);
			}
		}

		public Command CreateCommand(byte commandId)
		{
			var stream = CreateStreamInternal();

			return new Command(stream, commandId);
		}

		public void SendCommand(ushort streamId, Command command)
		{
			var isAtEndPosition = true;
			var offset = new StreamOffset(0);
			var channel = new ChannelID(streamId);

			var commandPayload = command.Stream.Close();

			var commandStream = CreateStreamInternal();

			commandStream.WriteLength((ushort)(commandPayload.Length + 1));
			commandStream.WriteUint8(command.CommandId);
			commandStream.WriteOctets(commandPayload);
			var wrappedCommandPayload = commandStream.Close();

			var streamData = new StreamDataCommand(channel, wrappedCommandPayload, isAtEndPosition, offset);

			var streamDataStream = CreateStream(Commands.StreamData);

			StreamDataSerializer.SerializeStreamData(streamDataStream, streamData);

			SendPacket(streamDataStream);
		}

		private bool IsConnected
		{
			get
			{
				return status == ConnectionStatus.Connected;
			}
		}

		void SendSubscribe(uint streamId, uint offset)
		{
			// log.Debug("SendSubscribe! {0} {1}", streamId, offset);
			var stream = CreateStream(Commands.SubscribeStream);
			stream.WriteUint8(1);
			stream.WriteUint32(streamId);
			const byte qos = 1;
			stream.WriteUint8(qos);
			stream.WriteUint32(offset);
			SendPacket(stream);
		}

		void SendLogin(string username, string password)
		{
			log.Debug("SendLogin {0} ****", username);
			var stream = CreateStream(Commands.Login);
			var login = new LoginCommand(username, password);
			LoginSerializer.SerializeLogin(stream, login);
			SendPacket(stream);
		}

		void SendPong(Timestamp echoedTime)
		{
			var stream = CreateStream(Commands.Pong);
			var pong = new PongCommand(Timestamp.Now(), echoedTime);

			PongSerializer.SerializePong(stream, pong);
			SendPacket(stream);
		}

		void SendPacket(IStreamWriter stream)
		{
			var octets = stream.Close();

			SendPacket(octets);
		}

		void SendPacket(byte[] octets)
		{
			connectionMaintainer.Write(octets);
		}

		void SetStatus(ConnectionStatus status)
		{
			log.Debug("Connection status:{0}", status);
			this.status = status;
		}

		void SetState(ConnectionState state)
		{
			log.Debug("Connection state:{0}", state);
			this.state = state;
		}

		private void OnPacketRead(HastyPacket packet)
		{
			InternalPacket(packet);
		}

		void InternalPacket(HastyPacket packet)
		{
			log.Debug("Received internal packet: {0}", packet);
			var octetReader = new OctetReader(packet.Octets, log);

			octetReader.ReadUint8();
			var streamReader = new StreamReader(octetReader, log);

			switch (packet.Command)
			{
			case Commands.ConnectResult:
				ConnectResult(streamReader);
				break;
			case Commands.LoginResult:
				LoginResult(streamReader);
				break;
			case Commands.StreamData:
				StreamData(streamReader);
				break;
			case Commands.Ping:
				Ping(streamReader);
				break;
			case Commands.Pong:
				Pong(streamReader);
				break;
			default:
				throw new Exception(string.Format("Unknown internal command {0}", packet.Command));
			}
		}

		void Ping(StreamReader streamReader)
		{
			log.Debug("onPing");
			PingCommand ping;
			PingDeserializer.DeserializePing(streamReader, out ping);

			SendPong(ping.SentTime);
		}

		void Pong(StreamReader streamReader)
		{
			log.Debug("OnPong");
			PongCommand pong;
			PongDeserializer.DeserializePong(streamReader, out pong);
			var now = Timestamp.Now();
			var latency = now - pong.EchoedTime;
			log.Debug("now:{0} echo:{1} sent:{2} Latency:{3}", now, pong.EchoedTime, pong.SentTime, latency);
		}

		void StreamData(IStreamReader streamReader)
		{
			var streamId = streamReader.ReadUint32();
			var streamOffset = streamReader.ReadUint32();
			var lastInStream = streamReader.ReadUint8();
			var payloadInPacket = streamReader.ReadLength();
			var octetCount = streamReader.RemainingOctetCount;

			if (octetCount != payloadInPacket)
			{
				var e = new Exception(string.Format("ERROR: {0} {1}", octetCount, payloadInPacket));
				log.Exception(e);
			}
			// log.Debug("Stream id:{0:X} offset:{1} octetCount:{2}", streamId, streamOffset, octetCount);

			var alreadyWrittenOctetCount = streamStorage.FileSize(streamId);
			var streamDataOctets = streamReader.ReadOctets(octetCount);

			var startIndexInStreamDataOctets = alreadyWrittenOctetCount - streamOffset;

			if (startIndexInStreamDataOctets < 0)
			{
				log.Warning("We have been sent data with a 'gap'! Can not write.");
				return;
			}

			if (startIndexInStreamDataOctets >= streamDataOctets.Length)
			{
				log.Warning("This is old news and contains no new octets. Skipping...");
				return;
			}
			var startWritePosition = (uint)startIndexInStreamDataOctets;
			var octetsToWrite = (uint)(streamDataOctets.Length - startIndexInStreamDataOctets);
			// log.Debug("Streamx id:{0:X} startIndexInData:{1} octetsToWrite:{2}", streamId, startIndexInStreamDataOctets, octetsToWrite);
			streamStorage.Append(streamId, streamDataOctets, startWritePosition, octetsToWrite);
			InternalOnStreamData(streamId, streamDataOctets, startWritePosition, octetsToWrite);
		}

		void
		InternalOnStreamData(uint streamId, byte[] octets, uint streamOffset, uint octetsToWrite)
		{
			//log.Warning("InternalOnStreamData id:{0:X} streamOffset {1} octetsToWrite {2}", streamId, streamOffset, octetsToWrite);
			// log.Warning("InternalOnStreamData channel:{0} octets:{1}", streamId, Debug.OctetBufferDebug.OctetsToHex(octets));
			StreamHandler streamHandler;
			var worked = chunkHandlers.TryGetValue(streamId, out streamHandler);

			if (!worked)
			{
				log.Warning("We don't have any chunk handlers for stream {0}", streamId);
				return;
			}
			var buf = new byte[octetsToWrite];
			Buffer.BlockCopy(octets, (int)streamOffset, buf, 0, (int)octetsToWrite);
			streamHandler.Receive(buf);
		}

		void ConnectResult(IStreamReader stream)
		{
			log.Debug("ConnectResult");
			var result = stream.ReadUint8();
			switch (result)
			{
			case 1:
				log.Debug("Connect was ok");
				OnConnected();
				break;
			case 2:
				log.Debug("Redirect");
				var urlString = stream.ReadString();
				var url = new Uri(urlString);
				connectionMaintainer.SwitchToTemporaryHost(url);
				break;
			}
		}

		void LoginResult(IStreamReader stream)
		{
			log.Debug("LoginResult");
			var result = stream.ReadUint8();
			switch (result)
			{
			case 1:
				log.Debug("Login was ok");
				OnLoggedIn(stream);
				break;
			default:
				log.Debug("Login failed");
				connectionMaintainer.Disconnect("Login Failed");
				break;
			}
		}
	}
}

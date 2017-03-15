using System;
using System.Collections.Generic;
using Hasty.Client.Connection;
using Hasty.Client.Octet;
using Hasty.Client.Packet;
using Hasty.Client.PacketHandler;
using Hasty.Client.Storage;

namespace Hasty.Client.Api
{
	public class HastyClient
	{
		ConnectionMaintainer connectionMaintainer;
		ExecutorPacketReceiver receiver;

		ConnectionStatus status;
		IStreamStorage streamStorage;
		uint subscribingChannel;
		Dictionary<uint, StreamHandler> streamHandlers = new Dictionary<uint, StreamHandler>();
		CommandDefinitions definitions;

		ILog log;

		public HastyClient(Uri serverUrl, string realm, CommandDefinitions definitions, string baseDir, ILog log)
		{
			this.log = log.CreateLog(typeof(HastyClient));
			var storage = new StreamStorage(baseDir, log);
			streamStorage = storage;
			this.definitions = definitions;

			SetStatus(ConnectionStatus.Idle);
			connectionMaintainer = new ConnectionMaintainer(serverUrl, realm, log);
			connectionMaintainer.OnPacketRead += OnPacketRead;
			connectionMaintainer.OnDisconnect += OnDisconnect;
			connectionMaintainer.OnConnecting += OnConnecting;
			connectionMaintainer.OnConnected += OnConnected;
			connectionMaintainer.Start();
		}

		void OnDisconnect()
		{
			SetStatus(ConnectionStatus.Disconnected);
		}

		void OnConnecting()
		{
			SetStatus(ConnectionStatus.Connecting);
		}

		void OnConnected()
		{
			SetStatus(ConnectionStatus.Connected);

			if (subscribingChannel != 0)
			{
				var offset = (uint)0;
				SendSubscribe(subscribingChannel, offset);
			}
		}

		IStreamWriter CreateStream(byte command)
		{
			var octetWriter = new OctetWriter();
			var streamWriter = new StreamWriter(octetWriter);

			streamWriter.WriteUint8(command);

			return streamWriter;
		}

		public void Subscribe(uint streamId, object target)
		{
			var streamHandler = new StreamHandler(target, definitions, log);

			streamHandlers.Add(streamId, streamHandler);
			subscribingChannel = streamId;
			var data = streamStorage.ReadAll(streamId);
			InternalOnStreamData(streamId, data, 0, (uint)data.Length);

			if (IsConnected)
			{
				SendSubscribe(streamId, (uint)data.Length);
			}
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
			log.Debug("SendSubscribe! {0} {1}", streamId, offset);
			var stream = CreateStream(Commands.SubscribeStream);
			stream.WriteUint8(1);
			stream.WriteUint32(streamId);
			const byte qos = 1;
			stream.WriteUint8(qos);
			stream.WriteUint32(offset);
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

		private void OnPacketRead(HastyPacket packet)
		{
			log.Debug("PacketRead:{0}", packet);

			if (packet.Command >= 128)
			{
				InternalPacket(packet);
			}
			else
			{
				ApplicationSpecificPacket(packet);
			}
		}

		void ApplicationSpecificPacket(HastyPacket packet)
		{
			receiver.ReceivePacket(packet);
		}

		void InternalPacket(HastyPacket packet)
		{
			var octetReader = new OctetReader(packet.Octets, log);

			octetReader.ReadUint8();
			var streamReader = new StreamReader(octetReader, log);

			switch (packet.Command)
			{
			case Commands.ConnectResult:
				ConnectResult(streamReader);
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
		}

		void Pong(StreamReader streamReader)
		{
			log.Debug("OnPong");
		}

		void StreamData(IStreamReader streamReader)
		{
			var streamId = streamReader.ReadUint32();
			var streamOffset = streamReader.ReadUint32();
			var payloadInPacket = streamReader.ReadLength();
			var octetCount = streamReader.RemainingOctetCount;

			if (octetCount != payloadInPacket)
			{
				var e = new Exception(string.Format("ERROR: {0} {1}", octetCount, payloadInPacket));
				log.Exception(e);
			}
			log.Debug("Stream id:{0:X} offset:{1} octetCount:{2}", streamId, streamOffset, octetCount);

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
			log.Debug("Streamx id:{0:X} startIndexInData:{1} octetsToWrite:{2}", streamId, startIndexInStreamDataOctets, octetsToWrite);
			streamStorage.Append(streamId, streamDataOctets, startWritePosition, octetsToWrite);
			InternalOnStreamData(streamId, streamDataOctets, startWritePosition, octetsToWrite);
		}

		void InternalOnStreamData(uint streamId, byte[] octets, uint streamOffset, uint octetsToWrite)
		{
			log.Debug("InternalOnStreamData id:{0:X} streamOffset {1} octetsToWrite {2}", streamId, streamOffset, octetsToWrite);

			StreamHandler streamHandler;
			var worked = streamHandlers.TryGetValue(streamId, out streamHandler);

			if (!worked)
			{
				log.Warning("Didn't work");
				return;
			}
			var buf = new byte[octetsToWrite];
			Buffer.BlockCopy(octets, (int)streamOffset, buf, 0, (int)octetsToWrite);
			log.Debug("Receive it");
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
				break;
			case 2:
				log.Debug("Redirect");
				var urlString = stream.ReadString();
				var url = new Uri(urlString);
				connectionMaintainer.SwitchToTemporaryHost(url);
				break;
			}
		}
	}
}

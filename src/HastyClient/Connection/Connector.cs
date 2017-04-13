using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Hasty.Client.Api;

namespace Hasty.Client.Connection
{
	public class Connector
	{
		static ILog log;
		public static Stream Connect(Uri serverUrl, ILog creator)
		{
			log = creator.CreateLog(typeof(Connector));
			IPHostEntry hostInfo = Dns.GetHostEntry(serverUrl.Host);
			IPAddress hostAddress = hostInfo.AddressList[0];

			var hostEndPoint = new IPEndPoint(hostAddress, serverUrl.Port);
			var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.NoDelay = true;
			try
			{
				socket.Connect(hostEndPoint);
			}
			catch (Exception e)
			{
				log.Exception(e);
				throw;
			}

			if (!socket.Connected)
			{
				var e = new Exception("Socket is not connected");
				log.Exception(e);
				throw e;
			}

			var stream = new SslStream(new NetworkStream(socket), true, (sender, certificate, chain, sslPolicyErrors) => { return true; });
			var certificateCollection = new X509Certificate2Collection();
			stream.AuthenticateAsClient(serverUrl.ToString(), certificateCollection, SslProtocols.Tls, false);

			return stream;
		}
	}
}

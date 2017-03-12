using System;
using System.Collections.Generic;
using Hasty;
using NUnit.Framework;
using System.Linq;

namespace test
{
	public class TestReceiver : IPacketReceiver
	{
		public List<Packet> packets = new List<Packet>();

		public void ReceivePacket(Packet packet)
		{
			Console.WriteLine("Received Packet:{0}", packet);
			packets.Add(packet);
		}
	}

	[TestFixture]
	public class TestReceiveStream
	{
		TestReceiver receiver;

		[SetUp]
		public void init()
		{
			receiver = new TestReceiver();
		}

		private ReceiveStream CreateStream(byte[] data)
		{
			var stream = new ReceiveStream(receiver);

			stream.Add(data);

			return stream;
		}

		private void ConsumeData(byte[] data)
		{
			CreateStream(data);
		}

		[Test]
		public void TestStream()
		{
			var data = new byte[] { 0x02, 0x01, 0x48, 0x01, 0x42 };

			ConsumeData(data);

			Assert.That(receiver.packets.Count, Is.EqualTo(2));
			Assert.That(receiver.packets.Last().Command, Is.EqualTo(0x42));
			Assert.That(receiver.packets.First().Command, Is.EqualTo(0x01));
		}

		[Test]
		public void TestIncompleteStreamFirst()
		{
			var data = new byte[] { 0xf2, 0x42, 0x42 };

			ConsumeData(data);

			Assert.That(receiver.packets.Count, Is.EqualTo(0));
		}

		[Test]
		public void TestIncompleteStreamSecond()
		{
			var data = new byte[] { 0x02, 0x01, 0x48, 0xfe, 0x42 };

			ConsumeData(data);

			Assert.That(receiver.packets.Count, Is.EqualTo(1));
			Assert.That(receiver.packets.First().Command, Is.EqualTo(0x01));
		}
	}
}

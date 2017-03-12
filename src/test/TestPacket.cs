using System;
using Hasty;
using NUnit.Framework;

namespace test
{
	[TestFixture]
	public class TestPacket
	{
		[Test]
		public void TestCompletePacket()
		{
			var data = new byte[] { 0x06, 0x01, 0x48, 0x65, 0x6C, 0x6C, 0x6F };
			int remainingOctets;
			var packet = PacketDecoder.Decode(data, 0, data.Length, out remainingOctets);

			Assert.That(0x01, Is.EqualTo(packet.Command));
		}

		[Test]
		public void TestIncompletePacket()
		{
			var data = new byte[] { 0x06, 0x01, 0x48, 0x65, 0x6C, 0x6C };
			int nextOffset;
			var packet = PacketDecoder.Decode(data, 0, data.Length, out nextOffset);

			Assert.That(nextOffset, Is.EqualTo(0));
			Assert.That(packet, Is.Null);
		}

		[Test]
		public void TestMultiplePackets()
		{
			var data = new byte[] { 0x02, 0x01, 0x48, 0x01, 0x42 };
			int nextOctetOffset;
			var packet = PacketDecoder.Decode(data, 0, data.Length, out nextOctetOffset);

			Assert.That(nextOctetOffset, Is.EqualTo(3));
			Assert.That(packet.Command, Is.EqualTo(0x01));
			Assert.That(packet.Length, Is.EqualTo(0x02));

			int nextOctetOffset2;
			var packet2 = PacketDecoder.Decode(data, nextOctetOffset, data.Length - nextOctetOffset, out nextOctetOffset2);
			Assert.That(nextOctetOffset2, Is.EqualTo(5));
			Assert.That(packet2.Command, Is.EqualTo(0x42));
			Assert.That(packet2.Length, Is.EqualTo(0x01));
		}
	}
}

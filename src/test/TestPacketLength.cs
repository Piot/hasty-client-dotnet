using NUnit.Framework;
using Hasty;

namespace test
{
	[TestFixture]
	public class TestPacketLength
	{
		[Test]

		public void TestLower()
		{
			var octets = new byte[] { 0x73 };

			int octetsUsed = 0;
			var value = PacketLength.Convert(octets, 0, out octetsUsed);

			Assert.That(1, Is.EqualTo(octetsUsed));
			Assert.That(7 * 16 + 3, Is.EqualTo(value));
		}

		[Test]
		public void Test16bit()
		{
			var octets = new byte[] { 0x80, 0x01 };

			int octetsUsed = 0;
			var value = PacketLength.Convert(octets, 0, out octetsUsed);

			Assert.That(2, Is.EqualTo(octetsUsed));
			Assert.That(384, Is.EqualTo(value));
		}

		[Test]
		public void TestThreeOctetsLastShouldBeIgnored()
		{
			var octets = new byte[] { 0x80, 0x01, 0x30 };

			int octetsUsed = 0;
			var value = PacketLength.Convert(octets, 0, out octetsUsed);

			Assert.That(2, Is.EqualTo(octetsUsed));
			Assert.That(384, Is.EqualTo(value));
		}

		[Test]
		public void TestLastOctetZero()
		{
			var octets = new byte[] { 0xff, 0x00 };

			int octetsUsed = 0;
			var value = PacketLength.Convert(octets, 0, out octetsUsed);

			Assert.That(2, Is.EqualTo(octetsUsed));
			Assert.That(255, Is.EqualTo(value));
		}

		[Test]
		public void TestLastOctetNonZero()
		{
			var octets = new byte[] { 0xff, 0x02 };

			int octetsUsed = 0;
			var value = PacketLength.Convert(octets, 0, out octetsUsed);

			Assert.That(2, Is.EqualTo(octetsUsed));
			Assert.That(255 + 2 * 256, Is.EqualTo(value));
		}
	}
}

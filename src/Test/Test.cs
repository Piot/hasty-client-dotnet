using NUnit.Framework;
using System;
using Hasty.Client.Storage;

namespace Test
{
	[TestFixture()]
	public class Test
	{
		[Test()]
		public void TestCase()
		{
			var data = new Preferences.Data
			{
				Endpoint = "http://some.strange.route/kalle123?", Channel = 1234, Realm = "test-stuff-23"
			};

			var s = Preferences.Serialize(data);
			var data2 = Preferences.Deserialize(s);

			Assert.That(data.Channel, Is.EqualTo(data2.Channel));
			Assert.That(data.Endpoint, Is.EqualTo(data2.Endpoint));
		}
	}
}

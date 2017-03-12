using Hasty;
using NUnit.Framework;

namespace test
{
	public class GameState
	{
		int score;
		string answeredName;

		public string AnsweredName
		{
			get
			{
				return answeredName;
			}
		}

		public int Score
		{
			get
			{
				return score;
			}
		}

		public void AnsweredCorrectly_1(IStreamReader stream)
		{
			answeredName = stream.ReadString();
			score += 1;
		}
	}

	public class TestCommandExecutor
	{
		CommandExecutor<GameState> executor;
		GameState state;

		public TestCommandExecutor()
		{
		}

		private ReceiveStream CreateStream(byte[] data)
		{
			var executorPacketReceiver = new ExecutorPacketReceiver(executor);
			var stream = new ReceiveStream(executorPacketReceiver);

			stream.Add(data);

			return stream;
		}

		private void ConsumeData(byte[] data)
		{
			CreateStream(data);
		}

		[SetUp]
		public void Init()
		{
			state = new GameState();
			executor = new CommandExecutor<GameState>(state);
			executor.Define(32, "AnsweredCorrectly");
		}

		[Test]
		public void TestExecutor()
		{
			var data = new byte[] { 0x03, 0x20, 0x01, 0x30 };

			ConsumeData(data);
			Assert.That(state.Score, Is.EqualTo(1));
			Assert.That(state.AnsweredName, Is.EqualTo("0"));
		}

		[Test]
		public void TestExecutorWithUtf8()
		{
			var data = new byte[] { 0x08, 0x20, 0x06, 0x31, 0xC2, 0xA9, 0x41, 0xC3, 0xA5 };

			ConsumeData(data);
			Assert.That(state.Score, Is.EqualTo(1));
			ConsumeData(data);
			Assert.That(state.Score, Is.EqualTo(2));
			Assert.That(state.AnsweredName, Is.EqualTo("1©Aå"));
		}
	}
}

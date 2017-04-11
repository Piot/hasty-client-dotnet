namespace Hasty
{
	public static class Commands
	{
		public const byte NOP = 0x80;
		public const byte PublishStream = 0x81;
		public const byte SubscribeStream = 0x82;
		public const byte UnsubscribeStream = 0x83;
		public const byte CreateStream = 0x84;
		public const byte CreateStreamResult = 0x85;
		public const byte StreamData = 0x86;
		public const byte Connect = 0x87;
		public const byte ConnectResult = 0x88;
		public const byte Ping = 0x89;
		public const byte Pong = 0x8A;
		public const byte Login = 0x8B;
		public const byte LoginResult = 0x8C;
	}
}

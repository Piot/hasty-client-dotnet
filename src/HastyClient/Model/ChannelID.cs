namespace Hasty
{
	public class ChannelID
	{
		ushort id;

		public ChannelID(ushort id)
		{
			this.id = id;
		}

		public override string ToString()
		{
			return string.Format("[ChannelID {0}]", id);
		}

		public ushort Raw
		{
			get
			{
				return id;
			}
		}
	}
}

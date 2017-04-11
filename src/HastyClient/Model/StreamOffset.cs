namespace Hasty
{
	public class StreamOffset
	{
		ushort id;

		public StreamOffset(ushort id)
		{
			this.id = id;
		}

		public override string ToString()
		{
			return string.Format("[StreamOffset {0}]", id);
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

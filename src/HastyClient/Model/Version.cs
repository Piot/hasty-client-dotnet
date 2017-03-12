namespace Hasty.Client.Model
{
	public class Version
	{
		byte major;
		byte minor;
		byte patch;

		private Version()
		{
		}

		public Version(byte major, byte minor, byte patch)
		{
			this.major = major;
			this.minor = minor;
			this.patch = patch;
		}

		public override string ToString()
		{
			return string.Format("[Version {0}.{1}.{2}]", major, minor, patch);
		}

		public byte Major
		{
			get
			{
				return major;
			}
		}
		public byte Minor
		{
			get
			{
				return minor;
			}
		}
		public byte Patch
		{
			get
			{
				return patch;
			}
		}
	}
}

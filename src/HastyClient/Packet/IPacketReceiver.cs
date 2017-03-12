namespace Hasty.Client.Packet
{
	public interface IPacketReceiver
	{
		void ReceivePacket(HastyPacket packet);
	}
}

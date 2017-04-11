using Hasty.Client.Model;

namespace Hasty.Client.Serializer
{
	public class LoginSerializer
	{
		public static void SerializeLogin(IStreamWriter writer, LoginCommand login)
		{
			writer.WriteString(login.Username);
			writer.WriteString(login.Password);
		}
	}
}

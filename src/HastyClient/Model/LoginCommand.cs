using System;
namespace Hasty.Client.Model
{
	public class LoginCommand
	{
		public LoginCommand(string username, string password)
		{
			Username = username;
			Password = password;
		}

		public string Username
		{
			get;

			private set;
		}

		public string Password
		{
			get;

			private set;
		}


	}
}

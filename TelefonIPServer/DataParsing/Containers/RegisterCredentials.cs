namespace DataParsing.Containers
{
    public class RegisterCredentials : ICredentials
    {
        public string Login { get; }
        public string Password { get; }
        public string Email { get; }

        public RegisterCredentials(string login, string password, string email)
        {
            Login = login;
            Password = password;
            Email = email;
        }
    }
}
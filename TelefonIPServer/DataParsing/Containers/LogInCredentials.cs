namespace DataParsing.Containers
{
    public sealed class LogInCredentials : ICredentials
    {
        public string Login { get; }
        public string Password { get; }

        public LogInCredentials(string login, string password)
        {
            Login = login;
            Password = password;
        }
    }
}
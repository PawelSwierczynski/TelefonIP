using ClientServerCommunicationProtocol;
using Security;

namespace CSCPClient
{
    public sealed class ServerInteraction
    {
        private readonly HashComputer hashComputer;

        public ServerInteraction()
        {
            hashComputer = new HashComputer();
        }

        public void SendLogInMessage(TCPClient tcpClient, string login, string password)
        {
            string hashedPassword = hashComputer.ComputeHashUsingSHA512(password);

            tcpClient.SendMessage(Command.LogInRequest, login + ";" + hashedPassword);
        }

        public void SendRegisterMessage(TCPClient tcpClient, string login, string password, string email)
        {
            string hashedPassword = hashComputer.ComputeHashUsingSHA512(password);

            tcpClient.SendMessage(Command.RegisterRequest, login + ";" + email + ";" + hashedPassword);
        }
    }
}
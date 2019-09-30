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

        public void SendRetrieveContacts(TCPClient tcpClient)
        {
            tcpClient.SendMessage(Command.ContactsRequest, "");
        }

        public void SendMoveContact(TCPClient tcpClient, string contactLogin, ContactType requestedContactType)
        {
            tcpClient.SendMessage(Command.MoveContactRequest, (int)requestedContactType + contactLogin);
        }

        public void SendDeleteContact(TCPClient tcpClient, string contactLogin)
        {
            tcpClient.SendMessage(Command.DeleteContactRequest, contactLogin);
        }

        public void SendAddContact(TCPClient tcpClient, string contactLogin)
        {
            tcpClient.SendMessage(Command.AddContactRequest, contactLogin);
        }
    }
}
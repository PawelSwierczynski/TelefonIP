namespace TelefonIPServer {
    public sealed class Server {
        private const int PortNumber = 17000;

        public static void Main()
        {
            TCPServer tcpServer = new TCPServer(PortNumber);
            tcpServer.AcceptClients();
        }
    }
}
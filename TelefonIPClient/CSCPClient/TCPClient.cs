using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ClientServerCommunicationProtocol;

namespace CSCPClient
{
    public sealed class TCPClient
    {
        private volatile bool endConnection;
        private int identifier;
        private int identifierOfAwaitedResponse;
        private int userToken;
        private readonly TcpClient tcpClient;
        private CSCPPacket awaitedResponse;
        private readonly List<CSCPPacket> receivedMessages;
        private readonly List<CSCPPacket> sentMessages;
        private Thread messageReceiver;

        public TCPClient(string ipAddress, int portNumber)
        {
            endConnection = false;
            identifier = 0;
            identifierOfAwaitedResponse = 0;
            userToken = 0;
            tcpClient = new TcpClient();
            tcpClient.Connect(ipAddress, portNumber);
            awaitedResponse = null;
            receivedMessages = new List<CSCPPacket>();
            sentMessages = new List<CSCPPacket>();
        }

        public void Start()
        {
            messageReceiver = new Thread(() =>
            {
                StartReceivingMessages();
            });

            messageReceiver.Start();
        }

        private void StartReceivingMessages()
        {
            CSCPPacket message;

            using (StreamReader streamReader = new StreamReader(tcpClient.GetStream(), new UTF8Encoding(false)))
            {

                for (; ; )
                {
                    if (endConnection)
                    {
                        break;
                    }
                    else
                    {
                        message = new CSCPPacket(streamReader);

                        AnalyzeMessage(message);

                        receivedMessages.Add(message);
                    }
                }
            }
        }

        private void AnalyzeMessage(CSCPPacket message)
        {
            switch (message.Command)
            {
                case Command.EndConnectionAck:
                    endConnection = true;
                    tcpClient.Close();

                    break;
            }

            if (message.Identifier == identifierOfAwaitedResponse)
            {
                awaitedResponse = message;
            }
        }

        public void IncrementIdentifier()
        {
            if (identifier == int.MaxValue)
            {
                identifier = 0;
            }
            else
            {
                identifier++;
            }
        }

        public void SendMessage(Command command, string data)
        {
            CSCPPacket message = new CSCPPacket(identifier, command, userToken, data);

            identifierOfAwaitedResponse = identifier;

            IncrementIdentifier();

            sentMessages.Add(message);

            using (StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream(), new UTF8Encoding(false)))
            {
                streamWriter.Write(message.Serialize());
                streamWriter.Flush();
            }
        }

        public void Stop()
        {
            SendMessage(Command.EndConnection, "");
        }

        public CSCPPacket GetAwaitedResponse()
        {
            return awaitedResponse;
        }
    }
}
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace CCPClient
{
    public sealed class CCPServer
    {
        private readonly TcpListener TCPListener;
        private ICallReceiver callReceiver;

        public CCPServer(int portNumber)
        {
            TCPListener = new TcpListener(IPAddress.Any, portNumber);
        }

        public void SubscribeToReceiveCallMessages(ICallReceiver callReceiver)
        {
            this.callReceiver = callReceiver;
        }

        public void AcceptClients()
        {
            TCPListener.Start();

            for (; ; )
            {
                TcpClient tcpClient = TCPListener.AcceptTcpClient();

                Thread clientThread = new Thread(() =>
                {
                    CommunicateWithClient(tcpClient);
                });

                clientThread.Start();
            }
        }

        private void CommunicateWithClient(TcpClient tcpClient)
        {
            bool endConnection = false;
            char[] message = new char[1];

            StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream(), new UTF8Encoding(false));
            StreamReader streamReader = new StreamReader(tcpClient.GetStream(), new UTF8Encoding(false));

            try
            {
                for (; ; )
                {
                    if (endConnection)
                    {
                        break;
                    }

                    streamReader.ReadBlock(message, 0, 1);

                    AnalyzeMessage(ref endConnection, message, streamWriter, tcpClient);
                }
            }
            catch (Exception)
            {

            }

            streamWriter.Dispose();
            streamReader.Dispose();
        }

        private void AnalyzeMessage(ref bool endConnection, char[] message, StreamWriter streamWriter, TcpClient tcpClient)
        {
            CCPCommand command = (CCPCommand)int.Parse(new string(message));

            
        }
    }
}
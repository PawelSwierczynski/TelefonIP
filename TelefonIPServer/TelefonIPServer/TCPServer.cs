﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ClientServerCommunicationProtocol;
using DataParsing;
using DataParsing.Containers;
using TelefonIPServer.Accounts;

namespace TelefonIPServer
{
    public sealed class TCPServer
    {
        private readonly AccountsManager accountsManager;
        private readonly DataParser dataParser;
        private readonly TcpListener TCPListener;

        public TCPServer(int portNumber)
        {
            accountsManager = new AccountsManager();
            dataParser = new DataParser();
            TCPListener = new TcpListener(IPAddress.Any, portNumber);
        }

        public void AcceptClients()
        {
            TCPListener.Start();

            Console.WriteLine("Server started accepting clients.");

            for (; ; )
            {
                TcpClient tcpClient = TCPListener.AcceptTcpClient();

                Thread clientThread = new Thread(() => {
                    CommunicateWithClient(tcpClient);
                });

                clientThread.Start();
            }
        }

        private void CommunicateWithClient(TcpClient tcpClient)
        {
            bool endConnection = false;
            CSCPPacket message;

            Console.WriteLine("Client started a connection.");

            StreamWriter streamWriter = new StreamWriter(tcpClient.GetStream(), new UTF8Encoding(false));
            StreamReader streamReader = new StreamReader(tcpClient.GetStream(), new UTF8Encoding(false));

            for (; ; )
            {
                if (endConnection)
                {
                    break;
                }

                message = new CSCPPacket(streamReader);

                AnalyzeMessage(ref endConnection, message, streamWriter);
            }

            streamWriter.Dispose();
            streamReader.Dispose();

            Console.WriteLine("Client ended a connection.");
        }

        private void AnalyzeMessage(ref bool endConnection, CSCPPacket message, StreamWriter streamWriter)
        {
            switch (message.Command)
            {
                case Command.EndConnection:
                    endConnection = true;
                    ReplyMessage(message.Identifier, Command.EndConnectionAck, message.UserToken, "", streamWriter);
                    break;

                case Command.LogInRequest:
                    LogInCredentials logInCredentials = dataParser.ExtractLogInCredentials(message.Data);

                    if (accountsManager.IsLogInSuccessful(logInCredentials))
                    {

                    }
                    else
                    {
                        ReplyMessage(message.Identifier, Command.LogInInvalidCredentials, message.UserToken, "", streamWriter);
                    }

                    break;
            }
        }

        private void ReplyMessage(int identifier, Command command, int userToken, string data, StreamWriter streamWriter)
        {
            CSCPPacket message = new CSCPPacket(identifier, command, userToken, data);

            streamWriter.Write(message.Serialize());
            streamWriter.Flush();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ClientServerCommunicationProtocol;
using DataParsing.Containers;
using TelefonIPServer.Accounts;

namespace TelefonIPServer
{
    public sealed class TCPServer
    {
        private readonly AccountsManager accountsManager;
        private readonly DataParsing.DataParser dataParser;
        private readonly DatabaseInteraction databaseInteraction;
        private readonly TcpListener TCPListener;
        private readonly TokenGenerator tokenGenerator;
        private readonly ConcurrentDictionary<int, CallingUser> callingStates;

        public TCPServer(int portNumber)
        {
            accountsManager = new AccountsManager();
            dataParser = new DataParsing.DataParser();
            databaseInteraction = new DatabaseInteraction();
            TCPListener = new TcpListener(IPAddress.Any, portNumber);
            tokenGenerator = new TokenGenerator(new Random());
            callingStates = new ConcurrentDictionary<int, CallingUser>();
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

            try
            {
                for (; ; )
                {
                    if (endConnection)
                    {
                        break;
                    }

                    message = new CSCPPacket(streamReader);

                    AnalyzeMessage(ref endConnection, message, streamWriter, tcpClient);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            streamWriter.Dispose();
            streamReader.Dispose();

            Console.WriteLine("Client ended a connection.");
        }

        private void AnalyzeMessage(ref bool endConnection, CSCPPacket message, StreamWriter streamWriter, TcpClient tcpClient)
        {
            switch (message.Command)
            {
                case Command.EndConnection:
                    endConnection = true;

                    databaseInteraction.ClearToken(message.UserToken);

                    if (message.UserToken != 0)
                    {
                        CallingUser callingUser;

                        callingStates.TryRemove(message.UserToken, out callingUser);
                    }

                    ReplyMessage(message.Identifier, Command.EndConnectionAck, message.UserToken, "", streamWriter);
                    break;
                case Command.LogInRequest:
                    LogInCredentials logInCredentials = dataParser.ExtractLogInCredentials(message.Data);

                    if (accountsManager.IsLogInSuccessful(logInCredentials))
                    {
                        List<int> tokensInUse = databaseInteraction.RetrieveTokensInUse();
                        int token = tokenGenerator.RandomizeToken(tokensInUse);

                        string ipAddress = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address.ToString();

                        databaseInteraction.SaveUserTokenAndIP(logInCredentials.Login, token, ipAddress);

                        callingStates.TryAdd(token, new CallingUser("", CallingState.Idle));

                        ReplyMessage(message.Identifier, Command.LogInAccepted, token, "", streamWriter);
                    }
                    else
                    {
                        ReplyMessage(message.Identifier, Command.LogInInvalidCredentials, message.UserToken, "", streamWriter);
                    }

                    break;
                case Command.RegisterRequest:
                    RegisterCredentials registerCredentials = dataParser.ExtractRegisterCredentials(message.Data);

                    if (accountsManager.IsRegisterSuccessful(registerCredentials))
                    {
                        databaseInteraction.RegisterAccount(registerCredentials);

                        ReplyMessage(message.Identifier, Command.RegisterAccepted, message.UserToken, "", streamWriter);
                    }
                    else
                    {
                        ReplyMessage(message.Identifier, Command.RegisterCredentialsInUse, message.UserToken, "", streamWriter);
                    }

                    break;
                case Command.ContactsRequest:
                    string contactsData = databaseInteraction.GetContacts(message.UserToken);

                    ReplyMessage(message.Identifier, Command.ContactsSent, message.UserToken, contactsData, streamWriter);

                    break;
                case Command.MoveContactRequest:
                    databaseInteraction.MoveContact(message.UserToken, message.Data);

                    ReplyMessage(message.Identifier, Command.MoveContactAccepted, message.UserToken, "", streamWriter);
                    break;
                case Command.DeleteContactRequest:
                    databaseInteraction.DeleteContact(message.UserToken, message.Data);

                    ReplyMessage(message.Identifier, Command.DeleteContactAccepted, message.UserToken, "", streamWriter);
                    break;
                case Command.AddContactRequest:
                    if (databaseInteraction.DoesUserExist(message.Data))
                    {
                        if (!databaseInteraction.IsContactAlreadyInUse(message.UserToken, message.Data))
                        {
                            databaseInteraction.AddContact(message.UserToken, message.Data);

                            ReplyMessage(message.Identifier, Command.AddContactAccepted, message.UserToken, "", streamWriter);
                        }
                        else
                        {
                            ReplyMessage(message.Identifier, Command.AddContactAlreadyInUse, message.UserToken, "", streamWriter);
                        }
                    }
                    else
                    {
                        ReplyMessage(message.Identifier, Command.AddContactLoginNotFound, message.UserToken, "", streamWriter);
                    }
                    break;
                /*case Command.GetContactIPRequest:
                    if (databaseInteraction.IsCallPossible(message.UserToken, message.Data))
                    {
                        string contactIPAddress = databaseInteraction.GetContactUserIPAddress(message.Data);

                        ReplyMessage(message.Identifier, Command.GetContactIPSent, message.UserToken, contactIPAddress + ";" + message.Data, streamWriter);
                    }
                    else
                    {
                        ReplyMessage(message.Identifier, Command.GetContactIPInactive, message.UserToken, "", streamWriter);
                    }

                    break;*/
                case Command.StartRingingRequest:
                    if (databaseInteraction.IsCallPossible(message.UserToken, message.Data))
                    {
                        string contactToken = databaseInteraction.GetContactUserToken(message.Data);

                        callingStates[int.Parse(contactToken)].Token = message.UserToken.ToString();
                        callingStates[int.Parse(contactToken)].CallingState = CallingState.Ringing;

                        ReplyMessage(message.Identifier, Command.GetContactIPSent, message.UserToken, contactToken, streamWriter);
                    }
                    else
                    {
                        ReplyMessage(message.Identifier, Command.StartRingingRejected, message.UserToken, "", streamWriter);
                    }

                    break;
                case Command.GetIsSomebodyRingingRequest:
                    if (callingStates[message.UserToken].CallingState == CallingState.Ringing)
                    {
                        ReplyMessage(message.Identifier, Command.GetIsSomebodyRingingTrue, message.UserToken, callingStates[message.UserToken].Token, streamWriter);
                    }
                    else
                    {
                        ReplyMessage(message.Identifier, Command.GetIsSomebodyRingingFalse, message.UserToken, "", streamWriter);
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
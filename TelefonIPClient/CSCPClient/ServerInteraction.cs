﻿using ClientServerCommunicationProtocol;
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


        public void SendGetContactIP(TCPClient tcpClient, string contactToken)
        {
            tcpClient.SendMessage(Command.GetContactIPRequest, contactToken);
        }

        public void SendGetIsSomebodyRinging(TCPClient tcpClient)
        {
            tcpClient.SendSilentMessage(Command.GetIsSomebodyRingingRequest, "");
        }

        public void SendStartRinging(TCPClient tcpClient, string contactToken)
        {
            tcpClient.SendMessage(Command.StartRingingRequest, contactToken);
        }

        public void SendAcceptCall(TCPClient tcpClient, AudioCodec audioCodec)
        {
            tcpClient.SendMessage(Command.AcceptCallRequest, ((int)audioCodec).ToString());
        }

        public void SendDeclineCall(TCPClient tcpClient)
        {
            tcpClient.SendMessage(Command.DeclineCallRequest, "");
        }

        public void SendGetCallState(TCPClient tcpClient, string calledToken)
        {
            tcpClient.SendMessage(Command.GetCallStateRequest, calledToken);
        }

        public void SendEndCall(TCPClient tcpClient, string calledToken)
        {
            tcpClient.SendMessage(Command.EndCallRequest, calledToken);
        }

        public void SendResetCallState(TCPClient tcpClient, string calledToken)
        {
            tcpClient.SendMessage(Command.ResetCallStateRequest, calledToken);
        }

        public void SendGetCallStateAsCalled(TCPClient tcpClient)
        {
            tcpClient.SendMessageWithOwnTokenAsData(Command.GetCallStateRequest);
        }

        public void SendEndCallAsCalled(TCPClient tcpClient)
        {
            tcpClient.SendMessageWithOwnTokenAsData(Command.EndCallRequest);
        }

        public void SendResetCallStateAsCalled(TCPClient tcpClient)
        {
            tcpClient.SendMessageWithOwnTokenAsData(Command.ResetCallStateRequest);
        }
    }
}
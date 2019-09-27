using ClientServerCommunicationProtocol;

namespace CSCPClient
{
    public interface IMessageReceiver
    {
        void RetrieveAwaitedMessage(CSCPPacket message);
    }
}
namespace CCPClient
{
    public interface ICallReceiver
    {
        void RetrieveAwaitedCallMessage(CCPCommand command);
    }
}
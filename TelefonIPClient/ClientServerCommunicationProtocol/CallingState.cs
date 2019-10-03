namespace ClientServerCommunicationProtocol
{
    public enum CallingState
    {
        Idle,
        Ringing,
        ReadyForConnection,
        EndedRinging,
        CallAccepted,
        CallDeclined,
        EndedCall
    }
}
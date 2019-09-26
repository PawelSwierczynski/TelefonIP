namespace ClientServerCommunicationProtocol
{
    public enum Command
    {
        LogInRequest,
        LogInAccepted,
        LogInInvalidCredentials,
        EndConnection,
        EndConnectionAck
    }
}
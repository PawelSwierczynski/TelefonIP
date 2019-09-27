namespace ClientServerCommunicationProtocol
{
    public enum Command
    {
        LogInRequest,
        LogInAccepted,
        LogInInvalidCredentials,
        RegisterRequest,
        RegisterCredentialsInUse,
        RegisterAccepted,
        EndConnection,
        EndConnectionAck
    }
}
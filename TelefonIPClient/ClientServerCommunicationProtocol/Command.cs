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
        ContactsRequest,
        ContactsSent,
        AddContactRequest,
        AddContactAccepted,
        AddContactAlreadyInUse,
        AddContactLoginNotFound,
        MoveContactRequest,
        MoveContactAccepted,
        GetContactIPRequest,
        GetContactIPInactive,
        GetContactIPSent,
        DeleteContactRequest,
        DeleteContactAccepted,
        EndConnection,
        EndConnectionAck,
        EndConnectionDueToError
    }
}
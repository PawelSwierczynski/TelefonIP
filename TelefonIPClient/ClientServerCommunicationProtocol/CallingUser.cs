namespace ClientServerCommunicationProtocol
{
    public sealed class CallingUser
    {
        public string Token { get; set; }
        public CallingState CallingState { get; set; }

        public CallingUser(string token, CallingState callingState)
        {
            Token = token;
            CallingState = callingState;
        }
    }
}
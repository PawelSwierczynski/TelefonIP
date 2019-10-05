namespace ClientServerCommunicationProtocol
{
    public sealed class CallingUser
    {
        public string Token { get; set; }
        public CallingState CallingState { get; set; }
        public AudioCodec PreferedAudioCodec { get; set; }

        public CallingUser(string token, CallingState callingState, AudioCodec preferedAudioCodec)
        {
            Token = token;
            CallingState = callingState;
            PreferedAudioCodec = preferedAudioCodec;
        }
    }
}
namespace TelefonIPServer.Accounts
{
    public sealed class UserDatabaseSearchResult
    {
        public bool IsUserFound {
            get {
                return User != null;
            }
        }

        public Users User { get; set; }
    }
}
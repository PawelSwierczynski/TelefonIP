using System.Collections.Generic;
using System.Linq;
using DataParsing.Containers;
using TelefonIPServer;

namespace TelefonIPServer.Accounts
{
    public sealed class AccountsManager
    {
        private readonly DatabaseInteraction databaseInteraction;

        public AccountsManager()
        {
            databaseInteraction = new DatabaseInteraction();
        }

        private UserDatabaseSearchResult FindUserWithMatchingCredentials(LogInCredentials logInCredentials)
        {
            List<Users> usersWithMatchingCredentials = databaseInteraction.RetrieveUsersWithMatchingLogInCredentials(logInCredentials);

            if (usersWithMatchingCredentials.Count != 1)
            {
                return new UserDatabaseSearchResult();
            }
            else
            {
                return new UserDatabaseSearchResult
                {
                    User = usersWithMatchingCredentials.First()
                };
            }
        }

        public bool IsLogInSuccessful(LogInCredentials logInCredentials)
        {
            UserDatabaseSearchResult userDatabaseSearchResult = FindUserWithMatchingCredentials(logInCredentials);

            return userDatabaseSearchResult.IsUserFound;
        }

        public bool IsRegisterSuccessful(RegisterCredentials registerCredentials)
        {
            List<Users> usersWithMatchingCredentials = databaseInteraction.RetrieveUsersWithMatchingRegisterCredentials(registerCredentials);

            return usersWithMatchingCredentials.Count == 0;
        }
    }
}
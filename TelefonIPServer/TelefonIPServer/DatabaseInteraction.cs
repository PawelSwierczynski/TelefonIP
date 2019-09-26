using System.Collections.Generic;
using System.Linq;
using DataParsing.Containers;

namespace TelefonIPServer
{
    public sealed class DatabaseInteraction
    {
        public List<Users> RetrieveUsersWithMatchingCredentials(LogInCredentials logInCredentials)
        {
            List<Users> usersWithMatchingCredentials;

            using (var database = new TelefonIPDBEntities())
            {
                usersWithMatchingCredentials = (from user in database.Users
                                                where user.Login == logInCredentials.Login
                                                where user.Password == logInCredentials.Password
                                                select user).ToList();
            }

            return usersWithMatchingCredentials;
        }

        public List<int> RetrieveTokensInUse()
        {
            List<int?> allTokensInUse;

            using (var database = new TelefonIPDBEntities())
            {
                allTokensInUse = (from user in database.Users
                               select user.Token).ToList();
            }

            List<int> tokensInUse = new List<int>();

            foreach (var token in allTokensInUse)
            {
                if (token != null)
                {
                    tokensInUse.Add((int)token);
                }
            }

            return tokensInUse;
        }
    }
}
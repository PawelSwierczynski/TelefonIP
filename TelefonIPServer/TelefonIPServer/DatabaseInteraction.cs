using System.Collections.Generic;
using System.Linq;
using DataParsing.Containers;

namespace TelefonIPServer
{
    public sealed class DatabaseInteraction
    {
        public List<Users> RetrieveUsersWithMatchingLogInCredentials(LogInCredentials logInCredentials)
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

        public List<Users> RetrieveUsersWithMatchingRegisterCredentials(RegisterCredentials registerCredentials)
        {
            List<Users> usersWithMatchingCredentials;

            using (var database = new TelefonIPDBEntities())
            {
                usersWithMatchingCredentials = (from user in database.Users
                                                where user.Login == registerCredentials.Login ||
                                                      user.EmailAddress == registerCredentials.Email
                                                select user).ToList();
            }

            return usersWithMatchingCredentials;
        }

        public void RegisterAccount(RegisterCredentials registerCredentials)
        {
            using (var database = new TelefonIPDBEntities())
            {
                int lastUserId = (from user in database.Users
                                  orderby user.UserID descending
                                  select user.UserID).FirstOrDefault();

                database.Users.Add(new Users()
                {
                    UserID = lastUserId + 1,
                    Login = registerCredentials.Login,
                    Password = registerCredentials.Password,
                    EmailAddress = registerCredentials.Email,
                });

                database.SaveChanges();
            }
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

        public void SaveUserTokenAndIP(string login, int token, string ipAddress)
        {
            using (var database = new TelefonIPDBEntities())
            {
                var userToReceiveToken = (from user in database.Users
                                          where user.Login == login
                                          select user).First();

                userToReceiveToken.Token = token;
                userToReceiveToken.IPAddress = ipAddress;

                database.SaveChanges();
            }
        }

        public void ClearToken(int token)
        {
            if (token != 0)
            {
                using (var database = new TelefonIPDBEntities())
                {
                    var userToGetTokenCleared = (from user in database.Users
                                                 where user.Token == token
                                                 select user).First();

                    userToGetTokenCleared.Token = 0;

                    database.SaveChanges();
                }
            }
        }
    }
}
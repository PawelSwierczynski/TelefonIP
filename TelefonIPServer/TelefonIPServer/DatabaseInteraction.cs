using System;
using System.Collections.Generic;
using System.Linq;
using DataParsing.Containers;
using ClientServerCommunicationProtocol;

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

        public string GetContacts(int token)
        {
            string contactsData = "";
            List<string> contacts = new List<string>();

            using (var database = new TelefonIPDBEntities())
            {
                int userID = (from user in database.Users
                              where user.Token == token
                              select user.UserID).First();

                contacts = (from contact in database.Contacts
                            join contactUser in database.Users on contact.ContactUserID equals contactUser.UserID
                            where contact.UserID == userID
                            select "" + contact.ContactType + contactUser.Login).ToList();
            }

            if (contacts.Count > 0)
            {
                foreach (var contact in contacts)
                {
                    contactsData += contact + ";";
                }

                return contactsData.Substring(0, contactsData.Length - 1);
            }
            else
            {
                return "";
            }
        }

        public void MoveContact(int token, string contactData)
        {
            DataParser dataParser = new DataParser();

            Contact contact = dataParser.RetrieveContact(contactData);

            using (var database = new TelefonIPDBEntities())
            {
                var userID = (from user in database.Users
                              where user.Token == token
                              select user.UserID).First();

                var contactUserID = (from user in database.Users
                                     where user.Login == contact.Name
                                     select user.UserID).First();

                var changedContact = (from contactToBeChanged in database.Contacts
                                      where contactToBeChanged.UserID == userID &&
                                      contactToBeChanged.ContactUserID == contactUserID
                                      select contactToBeChanged).First();

                changedContact.ContactType = (int)contact.ContactType;

                database.SaveChanges();
            }
        }

        public void DeleteContact(int token, string login)
        {
            using (var database = new TelefonIPDBEntities())
            {
                var userID = (from user in database.Users
                              where user.Token == token
                              select user.UserID).First();

                var contactUserID = (from user in database.Users
                                     where user.Login == login
                                     select user.UserID).First();

                var contactID = (from contact in database.Contacts
                                 where contact.UserID == userID &&
                                 contact.ContactUserID == contactUserID
                                 select contact.ContactID).SingleOrDefault();

                var attachedContact = new Contacts() { ContactID = contactID };

                database.Contacts.Attach(attachedContact);
                database.Contacts.Remove(attachedContact);

                database.SaveChanges();
            }
        }

        public bool IsContactAlreadyInUse(int token, string login)
        {
            List<Contacts> contacts;

            using (var database = new TelefonIPDBEntities())
            {
                var userID = (from user in database.Users
                              where user.Token == token
                              select user.UserID).First();

                var contactUserID = (from user in database.Users
                                     where user.Login == login
                                     select user.UserID).First();

                contacts = (from contact in database.Contacts
                            where contact.UserID == userID &&
                            contact.ContactUserID == contactUserID
                            select contact).ToList();
            }

            return contacts.Count > 0;
        }

        public bool DoesUserExist(string login)
        {
            bool doesUserExist;

            using (var database = new TelefonIPDBEntities())
            {
                doesUserExist = (from user in database.Users
                                 where user.Login == login
                                 select user).ToList().Count > 0;
            }

            return doesUserExist;
        }

        public void AddContact(int token, string login)
        {
            using (var database = new TelefonIPDBEntities())
            {
                var contactID = (from contact in database.Contacts
                                 orderby contact.ContactID descending
                                 select contact.ContactID).SingleOrDefault() + 1;

                var userID = (from user in database.Users
                              where user.Token == token
                              select user.UserID).Single();

                var contactUserID = (from user in database.Users
                                     where user.Login == login
                                     select user.UserID).Single();

                Contacts newContact = new Contacts()
                {
                    ContactID = contactID,
                    UserID = userID,
                    ContactUserID = contactUserID,
                    ContactType = (int)ContactType.Contact,
                    Timestamp = DateTime.Now
                };

                database.Contacts.Add(newContact);

                database.SaveChanges();
            }
        }

        public bool IsCallPossible(int token, string contactLogin)
        {
            bool isCallingUserBlocked;
            bool isCalledUserActive;

            using (var database = new TelefonIPDBEntities())
            {
                var callingUserID = (from user in database.Users
                                     where user.Token == token
                                     select user.UserID).Single();

                var calledUserID = (from user in database.Users
                                    where user.Login == contactLogin
                                    select user.UserID).Single();

                isCallingUserBlocked = (from contact in database.Contacts
                                        where contact.ContactID == callingUserID &&
                                        contact.UserID == calledUserID &&
                                        contact.ContactType == 2
                                        select contact).ToList().Count > 0;

                isCalledUserActive = (from user in database.Users
                                      where user.UserID == calledUserID
                                      select user).Single().Token != 0;
            }

            return !isCallingUserBlocked && isCalledUserActive;
        }

        public string GetContactUserIPAddress(string contactToken)
        {
            string ipAddress;

            using (var database = new TelefonIPDBEntities())
            {
                ipAddress = (from user in database.Users
                             where user.Token == int.Parse(contactToken)
                             select user.IPAddress).Single();
            }

            return ipAddress;
        }

        public string GetContactUserToken(string contactLogin)
        {
            string contactToken;

            using (var database = new TelefonIPDBEntities())
            {
                contactToken = (from user in database.Users
                                where user.Login == contactLogin
                                select user.Token).Single().ToString();
            }

            return contactToken;
        }

        public string GetContactLogin(string contactToken)
        {
            string contactLogin;

            using (var database = new TelefonIPDBEntities())
            {
                contactLogin = (from user in database.Users
                                where user.Token == int.Parse(contactToken)
                                select user.Login).Single();
            }

            return contactLogin;
        }
    }
}
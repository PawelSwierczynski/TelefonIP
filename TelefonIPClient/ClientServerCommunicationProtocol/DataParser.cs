using System.Collections.Generic;

namespace ClientServerCommunicationProtocol
{
    public sealed class DataParser
    {
        private const char DATA_DELIMITER = ';';
        private const int CONTACT_TYPE_STARTING_INDEX = 0;
        private const int CONTACT_TYPE_LENGTH = 1;
        private const int CONTACT_NAME_STARTING_INDEX = 1;


        public Contact RetrieveContact(string contactString)
        {
            ContactType contactType = (ContactType)int.Parse(contactString.Substring(CONTACT_TYPE_STARTING_INDEX, CONTACT_TYPE_LENGTH));
            string contactName = contactString.Substring(CONTACT_NAME_STARTING_INDEX);

            return new Contact(contactName, contactType);
        }

        public List<Contact> RetrieveContacts(string contactsData)
        {
            List<Contact> contacts = new List<Contact>();

            if (contactsData != null)
            {
                string[] contactsStrings = contactsData.Split(DATA_DELIMITER);

                foreach (var contact in contactsStrings)
                {
                    contacts.Add(RetrieveContact(contact));
                }
            }

            return contacts;
        }
    }
}
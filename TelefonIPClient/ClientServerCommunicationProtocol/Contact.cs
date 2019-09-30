namespace ClientServerCommunicationProtocol
{
    public sealed class Contact
    {
        public string Name { get; }
        public ContactType ContactType { get; set; }

        public Contact(string name, ContactType contactType)
        {
            Name = name;
            ContactType = contactType;
        }
    }
}
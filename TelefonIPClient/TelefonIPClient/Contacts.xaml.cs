using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CSCPClient;
using ClientServerCommunicationProtocol;

namespace TelefonIPClient
{
    public partial class Contacts : Window, IMessageReceiver
    {
        private bool isWindowSwitched;
        private readonly ServerInteraction serverInteraction;
        private readonly TCPClient tcpClient;
        private readonly DataParser dataParser;
        private List<Contact> contacts;
        private DispatcherTimer isSomebodyRingingTimer;

        public Contacts(ServerInteraction serverInteraction, TCPClient tcpClient, DispatcherTimer isSomebodyRingingTimer)
        {
            InitializeComponent();

            isWindowSwitched = false;
            this.serverInteraction = serverInteraction;
            this.tcpClient = tcpClient;
            this.tcpClient.SubscribeToReceiveAwaitedMessage(this);

            Closed += new EventHandler(Window_Closed);

            contacts = new List<Contact>();
            dataParser = new DataParser();

            serverInteraction.SendRetrieveContacts(tcpClient);

            this.isSomebodyRingingTimer = isSomebodyRingingTimer;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!isWindowSwitched)
            {
                tcpClient.Stop();
            }
        }

        public void RetrieveAwaitedMessage(CSCPPacket message)
        {
            switch (message.Command)
            {
                case Command.ContactsSent:
                    contacts = dataParser.RetrieveContacts(message.Data);

                    Dispatcher.Invoke(() =>
                    {
                        ResetContactList();
                    });

                    break;
                case Command.MoveContactAccepted:
                    serverInteraction.SendRetrieveContacts(tcpClient);
                    break;
                case Command.DeleteContactAccepted:
                    serverInteraction.SendRetrieveContacts(tcpClient);
                    break;
                case Command.GetIsSomebodyRingingTrue:
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        isWindowSwitched = true;
                        IncomingCall incomingCall = new IncomingCall(serverInteraction, tcpClient, isSomebodyRingingTimer, message.Data);
                        incomingCall.Show();
                        Close();
                    });

                    break;
                case Command.GetIsSomebodyRingingFalse:
                    break;
                case Command.StartRingingACK:
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        isWindowSwitched = true;
                        Calling calling = new Calling(serverInteraction, tcpClient, message.Data, isSomebodyRingingTimer);
                        calling.Show();
                        Close();
                    });

                    break;
                case Command.EndConnectionAck:
                    break;
                default:
                    MessageBox.Show("Natrafiono na nieobsługiwany rozkaz!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        private void NewContactButton_Click(object sender, RoutedEventArgs e)
        {
            isWindowSwitched = true;
            AddContact addContact = new AddContact(serverInteraction, tcpClient, isSomebodyRingingTimer);
            addContact.Show();
            Close();
        }

        private void DeleteContactButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteContactButton.IsEnabled = false;
            MoveContactButton.IsEnabled = false;
            MoveContactComboBox.IsEnabled = false;
            CallButton.IsEnabled = false;

            serverInteraction.SendDeleteContact(tcpClient, (string)((ListBoxItem)ContactsListBox.SelectedItem).Content);
        }

        private void MoveContactButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteContactButton.IsEnabled = false;
            MoveContactButton.IsEnabled = false;
            MoveContactComboBox.IsEnabled = false;
            CallButton.IsEnabled = false;

            string contactLogin = (string)((ListBoxItem)ContactsListBox.SelectedItem).Content;
            ContactType requestedContactType = (ContactType)(MoveContactComboBox.SelectedIndex + 1);

            serverInteraction.SendMoveContact(tcpClient, contactLogin, requestedContactType);
        }

        private void CallButton_Click(object sender, RoutedEventArgs e)
        {
            string contactLogin = (string)((ListBoxItem)ContactsListBox.SelectedItem).Content;

            //serverInteraction.SendGetContactIP(tcpClient, contactLogin);
            serverInteraction.SendStartRinging(tcpClient, contactLogin);
        }

        private void ContactsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContactListComboBox.SelectedItem != null)
            {
                DeleteContactButton.IsEnabled = true;
                MoveContactButton.IsEnabled = true;
                MoveContactComboBox.IsEnabled = true;
                CallButton.IsEnabled = true;
            }
        }

        private void ContactListComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResetContactList();
        }

        private void ResetContactList()
        {
            if (ContactsListBox != null)
            {
                ContactsListBox.Items.Clear();

                foreach (var contact in contacts)
                {
                    if (contact.ContactType == (ContactType)(ContactListComboBox.SelectedIndex + 1))
                    {
                        ContactsListBox.Items.Add(new ListBoxItem() { Content = contact.Name });
                    }
                }
            }
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            isWindowSwitched = true;
            MainMenu mainMenu = new MainMenu(serverInteraction, tcpClient, isSomebodyRingingTimer);
            mainMenu.Show();
            Close();
        }
    }
}
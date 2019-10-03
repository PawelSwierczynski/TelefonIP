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
    public partial class AddContact : Window, IMessageReceiver
    {
        private bool isWindowSwitched;
        private readonly ServerInteraction serverInteraction;
        private readonly TCPClient tcpClient;
        private readonly DispatcherTimer isSomebodyRingingTimer;

        public AddContact(ServerInteraction serverInteraction, TCPClient tcpClient, DispatcherTimer isSomebodyRingingTimer)
        {
            InitializeComponent();

            isWindowSwitched = false;
            this.serverInteraction = serverInteraction;
            this.tcpClient = tcpClient;
            this.tcpClient.SubscribeToReceiveAwaitedMessage(this);

            Closed += new EventHandler(Window_Closed);

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
                case Command.AddContactAccepted:
                    MessageBox.Show("Dodano kontakt.", "Dodano kontakt", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case Command.AddContactAlreadyInUse:
                    MessageBox.Show("Użytkownik o podanym loginie jest już dodany do listy kontaktów!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case Command.AddContactLoginNotFound:
                    MessageBox.Show("Nie odnaleziono użytkownika o podanym loginie!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
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
                case Command.EndConnectionAck:
                    break;
                default:
                    MessageBox.Show("Natrafiono na nieobsługiwany rozkaz!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        private void AddContactButton_Click(object sender, RoutedEventArgs e)
        {
            serverInteraction.SendAddContact(tcpClient, LoginTextBox.Text);
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            isWindowSwitched = true;
            Contacts contacts = new Contacts(serverInteraction, tcpClient, isSomebodyRingingTimer);
            contacts.Show();
            Close();
        }
    }
}
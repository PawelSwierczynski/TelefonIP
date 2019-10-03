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
    public partial class MainMenu : Window, IMessageReceiver
    {
        private bool isWindowSwitched;
        private readonly ServerInteraction serverInteraction;
        private readonly TCPClient tcpClient;
        private readonly DispatcherTimer isSomebodyRingingTimer;

        public MainMenu(ServerInteraction serverInteraction, TCPClient tcpClient)
        {
            InitializeComponent();

            isWindowSwitched = false;
            this.serverInteraction = serverInteraction;
            this.tcpClient = tcpClient;
            this.tcpClient.SubscribeToReceiveAwaitedMessage(this);

            Closed += new EventHandler(Window_Closed);

            isSomebodyRingingTimer = new DispatcherTimer();
            isSomebodyRingingTimer.Tick += SendRequestToCheckIfSomebodyIsRinging;
            isSomebodyRingingTimer.Interval = new TimeSpan(0, 0, 3);
            isSomebodyRingingTimer.Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!isWindowSwitched)
            {
                tcpClient.Stop();
            }
        }

        private void SendRequestToCheckIfSomebodyIsRinging(object sender, EventArgs e)
        {
            serverInteraction.SendGetIsSomebodyRinging(tcpClient);
        }

        public void RetrieveAwaitedMessage(CSCPPacket message)
        {
            switch (message.Command)
            {
                case Command.EndConnectionAck:
                    break;
                case Command.GetIsSomebodyRingingTrue:
                    MessageBox.Show("Ktoś dzwoni!", "HALOO", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case Command.GetIsSomebodyRingingFalse:
                    break;
                default:
                    MessageBox.Show("Natrafiono na nieobsługiwany rozkaz!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        private void ContactsButton_Click(object sender, RoutedEventArgs e)
        {
            isWindowSwitched = true;
            Contacts contacts = new Contacts(serverInteraction, tcpClient);
            contacts.Show();
            Close();
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
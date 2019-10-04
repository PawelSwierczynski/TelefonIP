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
    public partial class IncomingCall : Window, IMessageReceiver
    {
        private bool isWindowSwitched;
        private readonly ServerInteraction serverInteraction;
        private readonly TCPClient tcpClient;
        private DispatcherTimer isSomebodyRingingTimer;
        private readonly string callingUserToken;
        private string callingUserIP;
        private string callingUserLogin;

        public IncomingCall(ServerInteraction serverInteraction, TCPClient tcpClient, DispatcherTimer isSomebodyRingingTimer, string callingUserToken)
        {
            InitializeComponent();

            isWindowSwitched = false;
            this.serverInteraction = serverInteraction;
            this.tcpClient = tcpClient;
            this.tcpClient.SubscribeToReceiveAwaitedMessage(this);
            this.callingUserToken = callingUserToken;
            callingUserIP = "";
            callingUserLogin = "";

            Closed += new EventHandler(Window_Closed);

            this.isSomebodyRingingTimer = isSomebodyRingingTimer;
            this.isSomebodyRingingTimer.Stop();

            serverInteraction.SendGetContactIP(tcpClient, callingUserToken);
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
                case Command.GetContactIPSent:
                    string[] callingUserData = message.Data.Split(';');

                    callingUserIP = callingUserData[0];
                    callingUserLogin = callingUserData[1];

                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        CallingLabel.Content = callingUserLogin + " dzwoni.";
                    });

                    break;
                case Command.EndConnectionAck:
                    break;
                case Command.AcceptCallACK:
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        isWindowSwitched = true;
                        Call call = new Call(serverInteraction, tcpClient, callingUserToken, callingUserLogin, callingUserIP, isSomebodyRingingTimer);
                        call.Show();
                        Close();
                    });

                    break;
                case Command.DeclineCallACK:
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        isWindowSwitched = true;

                        isSomebodyRingingTimer.Start();
                        MainMenu mainMenu = new MainMenu(serverInteraction, tcpClient, isSomebodyRingingTimer);
                        mainMenu.Show();
                        Close();
                    });

                    break;
                default:
                    MessageBox.Show("Natrafiono na nieobsługiwany rozkaz!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        private void AcceptCallButton_Click(object sender, RoutedEventArgs e)
        {
            serverInteraction.SendAcceptCall(tcpClient);
        }

        private void DeclineCallButton_Click(object sender, RoutedEventArgs e)
        {
            serverInteraction.SendDeclineCall(tcpClient);
        }
    }
}
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
    public partial class Calling : Window, IMessageReceiver
    {
        private bool isWindowSwitched;
        private readonly ServerInteraction serverInteraction;
        private readonly TCPClient tcpClient;
        private string calledToken;
        private string calledLogin;
        private readonly DispatcherTimer isSomebodyRingingTimer;
        private readonly DispatcherTimer getCallStateTimer;

        public Calling(ServerInteraction serverInteraction, TCPClient tcpClient, string calledToken, string calledLogin, DispatcherTimer isSomebodyRingingTimer)
        {
            InitializeComponent();

            isWindowSwitched = false;
            this.serverInteraction = serverInteraction;
            this.tcpClient = tcpClient;
            this.tcpClient.SubscribeToReceiveAwaitedMessage(this);
            this.calledToken = calledToken;

            Closed += new EventHandler(Window_Closed);

            this.isSomebodyRingingTimer = isSomebodyRingingTimer;
            this.isSomebodyRingingTimer.Stop();

            this.calledLogin = calledLogin;
            CallingLabel.Content = "Dzwonisz do " + this.calledLogin + ".";

            getCallStateTimer = new DispatcherTimer();
            getCallStateTimer.Tick += SendRequestToGetCallState;
            getCallStateTimer.Interval = new TimeSpan(0, 0, 1);
            getCallStateTimer.Start();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!isWindowSwitched)
            {
                tcpClient.Stop();
            }
        }

        private void SendRequestToGetCallState(object sender, EventArgs e)
        {
            serverInteraction.SendGetCallState(tcpClient, calledToken);
        }

        public void RetrieveAwaitedMessage(CSCPPacket message)
        {
            switch (message.Command)
            {
                case Command.EndConnectionAck:
                    break;
                case Command.GetIsSomebodyRingingTrue:
                    break;
                case Command.GetIsSomebodyRingingFalse:
                    break;
                case Command.GetCallStateAccepted:
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        isWindowSwitched = true;
                        //Call call = new Call()
                        //call.Show();
                        Close();
                    });

                    break;
                case Command.GetCallStateDeclined:
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        MessageBox.Show(calledLogin + " odrzucił Twoją prośbę o rozmowę.", "Prośba odrzucona", MessageBoxButton.OK, MessageBoxImage.Information);

                        getCallStateTimer.Stop();
                        isSomebodyRingingTimer.Start();

                        isWindowSwitched = true;
                        Contacts contacts = new Contacts(serverInteraction, tcpClient, isSomebodyRingingTimer);
                        contacts.Show();
                        Close();
                    });

                    break;
                default:
                    MessageBox.Show("Natrafiono na nieobsługiwany rozkaz!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
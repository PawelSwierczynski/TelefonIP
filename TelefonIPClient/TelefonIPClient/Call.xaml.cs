using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using NAudioDemo.NetworkChatDemo;

namespace TelefonIPClient
{
    public partial class Call : Window, IMessageReceiver
    {
        private bool isWindowSwitched;
        private readonly ServerInteraction serverInteraction;
        private readonly TCPClient tcpClient;
        private string calledToken;
        private string calledLogin;
        private string calledIP;
        private readonly DispatcherTimer isSomebodyRingingTimer;
        private readonly DispatcherTimer getCallStateTimer;
        private TimeSpan callDuration;
        private NetworkAudioPlayer networkAudioPlayer;
        private NetworkAudioSender networkAudioSender;
        private INetworkChatCodec networkChatCodec;

        public Call(ServerInteraction serverInteraction, TCPClient tcpClient, string calledToken, string calledLogin, string calledIP, DispatcherTimer isSomebodyRingingTimer)
        {
            InitializeComponent();

            isWindowSwitched = false;
            this.serverInteraction = serverInteraction;
            this.tcpClient = tcpClient;
            this.tcpClient.SubscribeToReceiveAwaitedMessage(this);
            this.calledToken = calledToken;
            this.calledIP = calledIP;

            Closed += new EventHandler(Window_Closed);

            this.calledLogin = calledLogin;
            CallingLabel.Content = this.calledLogin;

            callDuration = new TimeSpan(0, 0, 0);
            TimeCounterLabel.Content = callDuration.ToString();

            getCallStateTimer = new DispatcherTimer();
            getCallStateTimer.Tick += SendRequestToGetCallState;
            getCallStateTimer.Interval = new TimeSpan(0, 0, 1);
            getCallStateTimer.Start();

            networkChatCodec = new G722ChatCodec();

            Connect();
        }

        private void Connect()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(calledIP), 17001);

            networkAudioPlayer = new NetworkAudioPlayer(networkChatCodec, new UdpAudioReceiver(17001));
            networkAudioSender = new NetworkAudioSender(networkChatCodec, 0, new UdpAudioSender(ipEndPoint));
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

            callDuration = callDuration.Add(new TimeSpan(0, 0, 1));

            TimeCounterLabel.Content = callDuration.ToString();
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
                    break;
                case Command.GetCallStateEnded:
                    //TODO
                    break;
                default:
                    MessageBox.Show("Natrafiono na nieobsługiwany rozkaz!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }
    }
}
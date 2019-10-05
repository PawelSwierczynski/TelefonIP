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
        private bool isUserCalling;

        public Call(ServerInteraction serverInteraction, TCPClient tcpClient, string calledToken, string calledLogin, string calledIP, DispatcherTimer isSomebodyRingingTimer, bool isUserCalling, AudioCodec preferedAudioCodec)
        {
            InitializeComponent();

            isWindowSwitched = false;
            this.serverInteraction = serverInteraction;
            this.tcpClient = tcpClient;
            this.tcpClient.SubscribeToReceiveAwaitedMessage(this);
            this.calledToken = calledToken;
            this.calledIP = calledIP;
            this.isSomebodyRingingTimer = isSomebodyRingingTimer;

            Closed += new EventHandler(Window_Closed);

            this.calledLogin = calledLogin;
            CallingLabel.Content = this.calledLogin;

            callDuration = new TimeSpan(0, 0, 0);
            TimeCounterLabel.Content = callDuration.ToString();

            this.isUserCalling = isUserCalling;

            getCallStateTimer = new DispatcherTimer();
            getCallStateTimer.Tick += SendRequestToGetCallState;
            getCallStateTimer.Interval = new TimeSpan(0, 0, 1);
            getCallStateTimer.Start();

            switch (preferedAudioCodec) {
                case AudioCodec.G711ALaw:
                    networkChatCodec = new AcmALawChatCodec();
                    break;
                case AudioCodec.G711MuLaw:
                    networkChatCodec = new AcmMuLawChatCodec();
                    break;
                case AudioCodec.G722:
                    networkChatCodec = new G722ChatCodec();
                    break;
                case AudioCodec.GSM610:
                    networkChatCodec = new Gsm610ChatCodec();
                    break;
                case AudioCodec.TrueSpeech:
                    networkChatCodec = new TrueSpeechChatCodec();
                    break;
            }

            Connect();
        }

        private void Connect()
        {
            IPEndPoint receiverEndPoint;
            IPEndPoint senderEndPoint;

            if (isUserCalling)
            {
                receiverEndPoint = new IPEndPoint(IPAddress.Any, 17001);
                senderEndPoint = new IPEndPoint(IPAddress.Parse(calledIP), 17002);
            }
            else
            {
                receiverEndPoint = new IPEndPoint(IPAddress.Any, 17002);
                senderEndPoint = new IPEndPoint(IPAddress.Parse(calledIP), 17001);
            }

            networkAudioPlayer = new NetworkAudioPlayer(networkChatCodec, new UdpAudioReceiver(receiverEndPoint));
            networkAudioSender = new NetworkAudioSender(networkChatCodec, 0, new UdpAudioSender(senderEndPoint));
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
            if (isUserCalling)
            {
                serverInteraction.SendGetCallState(tcpClient, calledToken);
            }
            else
            {
                serverInteraction.SendGetCallStateAsCalled(tcpClient);
            }

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
                    getCallStateTimer.Stop();

                    if (isUserCalling)
                    {
                        serverInteraction.SendResetCallState(tcpClient, calledToken);
                    }
                    else
                    {
                        serverInteraction.SendResetCallStateAsCalled(tcpClient);
                    }
                    break;
                case Command.ResetCallStateACK:
                case Command.EndCallACK:
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        networkAudioPlayer.Dispose();
                        networkAudioSender.Dispose();
                        networkChatCodec.Dispose();

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

        private void EndCallButton_Click(object sender, RoutedEventArgs e)
        {
            getCallStateTimer.Stop();

            if (isUserCalling)
            {
                serverInteraction.SendEndCall(tcpClient, calledToken);
            }
            else
            {
                serverInteraction.SendEndCallAsCalled(tcpClient);
            }
        }
    }
}
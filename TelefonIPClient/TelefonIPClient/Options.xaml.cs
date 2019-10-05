using System;
using System.Collections.Generic;
using System.IO;
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
    public partial class Options : Window, IMessageReceiver
    {
        private bool isWindowSwitched;
        private readonly ServerInteraction serverInteraction;
        private readonly TCPClient tcpClient;
        private readonly DispatcherTimer isSomebodyRingingTimer;

        public Options(ServerInteraction serverInteraction, TCPClient tcpClient, DispatcherTimer isSomebodyRingingTimer)
        {
            InitializeComponent();

            isWindowSwitched = false;
            this.serverInteraction = serverInteraction;
            this.tcpClient = tcpClient;
            this.tcpClient.SubscribeToReceiveAwaitedMessage(this);

            Closed += new EventHandler(Window_Closed);

            int preferedCodecIndex = int.Parse(File.ReadAllText("settings.ini"));
            AudioCodecsComboBox.SelectedIndex = preferedCodecIndex;

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
                case Command.EndConnectionAck:
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
                default:
                    MessageBox.Show("Natrafiono na nieobsługiwany rozkaz!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedAudioCodecIndex = AudioCodecsComboBox.SelectedIndex;

            File.WriteAllText("settings.ini", selectedAudioCodecIndex.ToString());
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
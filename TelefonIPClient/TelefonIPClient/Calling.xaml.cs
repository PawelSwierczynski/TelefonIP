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

        public Calling(ServerInteraction serverInteraction, TCPClient tcpClient, string calledToken)
        {
            InitializeComponent();

            isWindowSwitched = false;
            this.serverInteraction = serverInteraction;
            this.tcpClient = tcpClient;
            this.tcpClient.SubscribeToReceiveAwaitedMessage(this);
            this.calledToken = calledToken;

            Closed += new EventHandler(Window_Closed);
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
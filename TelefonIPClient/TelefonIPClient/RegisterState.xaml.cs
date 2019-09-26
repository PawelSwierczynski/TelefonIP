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

namespace TelefonIPClient
{
    public partial class RegisterState : Window
    {
        private bool isWindowSwitched;
        private readonly ServerInteraction serverInteraction;
        private readonly TCPClient tcpClient;

        public RegisterState(ServerInteraction serverInteraction, TCPClient tcpClient)
        {
            InitializeComponent();

            isWindowSwitched = false;
            this.serverInteraction = serverInteraction;
            this.tcpClient = tcpClient;

            Closed += new EventHandler(Window_Closed);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            isWindowSwitched = false;
            LogInState logInState = new LogInState(serverInteraction, tcpClient);
            logInState.Show();
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (!isWindowSwitched)
            {
                tcpClient.Stop();
            }
        }
    }
}
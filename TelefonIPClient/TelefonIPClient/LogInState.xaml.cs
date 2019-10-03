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
    public partial class LogInState : Window, IMessageReceiver
    {
        private bool isWindowSwitched;
        private readonly ServerInteraction serverInteraction;
        private readonly TCPClient tcpClient;

        public LogInState()
        {
            InitializeComponent();

            isWindowSwitched = false;
            serverInteraction = new ServerInteraction();
            tcpClient = new TCPClient("192.168.1.10", 17000);
            tcpClient.SubscribeToReceiveAwaitedMessage(this);
            tcpClient.Start();

            Closed += new EventHandler(Window_Closed);
        }

        public LogInState(ServerInteraction serverInteraction, TCPClient tcpClient)
        {
            InitializeComponent();

            isWindowSwitched = false;
            this.serverInteraction = serverInteraction;
            this.tcpClient = tcpClient;
            this.tcpClient.SubscribeToReceiveAwaitedMessage(this);

            Closed += new EventHandler(Window_Closed);
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            CredentialsValidator credentialsValidator = new CredentialsValidator();

            if (credentialsValidator.ValidateLogin(LoginTextBox.Text))
            {
                if (credentialsValidator.ValidatePassword(PasswordBox.Password))
                {
                    serverInteraction.SendLogInMessage(tcpClient, LoginTextBox.Text, PasswordBox.Password);
                }
                else
                {
                    MessageBox.Show("Podano za krótkie lub za długie hasło.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Podano za krótki lub za długi login.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            isWindowSwitched = true;
            RegisterState registerState = new RegisterState(serverInteraction, tcpClient);
            registerState.Show();
            Close();
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
                case Command.LogInAccepted:
                    Application.Current.Dispatcher.Invoke(delegate
                    {
                        isWindowSwitched = true;
                        MainMenu mainMenu = new MainMenu(serverInteraction, tcpClient);
                        mainMenu.Show();
                        Close();
                    });

                    break;
                case Command.LogInInvalidCredentials:
                    MessageBox.Show("Podano nieprawidłowy login lub hasło.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case Command.EndConnectionAck:
                    break;
                default:
                    MessageBox.Show("Natrafiono na nieobsługiwany rozkaz!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }
    }
}
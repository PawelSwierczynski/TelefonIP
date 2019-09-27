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
    public partial class RegisterState : Window, IMessageReceiver
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
            this.tcpClient.SubscribeToReceiveAwaitedMessage(this);

            Closed += new EventHandler(Window_Closed);
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            CredentialsValidator credentialsValidator = new CredentialsValidator();

            if (credentialsValidator.ValidateLogin(LoginTextBox.Text))
            {
                if (credentialsValidator.ValidatePassword(PasswordBox.Password))
                {
                    if (credentialsValidator.ValidateEmail(EmailTextBox.Text))
                    {
                        serverInteraction.SendRegisterMessage(tcpClient, LoginTextBox.Text, PasswordBox.Password, EmailTextBox.Text);
                    }
                    else
                    {
                        MessageBox.Show("Podano za krótki lub za długi adres e-mail.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
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

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            isWindowSwitched = true;
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

        public void RetrieveAwaitedMessage(CSCPPacket message)
        {
            switch (message.Command)
            {
                case Command.RegisterAccepted:
                    MessageBox.Show("Zarejestrowano nowe konto.", "Rejestracja", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case Command.RegisterCredentialsInUse:
                    MessageBox.Show("Podano istniejące w bazie dane!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
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
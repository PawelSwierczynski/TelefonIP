﻿using System;
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
    public partial class LogInState : Window
    {
        private bool isWindowSwitched;
        private readonly ServerInteraction serverInteraction;
        private readonly TCPClient tcpClient;

        public LogInState()
        {
            InitializeComponent();

            isWindowSwitched = false;
            serverInteraction = new ServerInteraction();
            tcpClient = new TCPClient("127.0.0.1", 17000);
            tcpClient.Start();

            Closed += new EventHandler(Window_Closed);
        }

        public LogInState(ServerInteraction serverInteraction, TCPClient tcpClient)
        {
            InitializeComponent();

            isWindowSwitched = false;
            this.serverInteraction = serverInteraction;
            this.tcpClient = tcpClient;
        }

        private void LogInButton_Click(object sender, RoutedEventArgs e)
        {
            serverInteraction.SendLogInMessage(tcpClient, LoginTextBox.Text, PasswordBox.Password);
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
    }
}
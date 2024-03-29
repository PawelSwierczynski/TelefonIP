﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace NAudioDemo.NetworkChatDemo
{
    public class UdpAudioReceiver : IAudioReceiver
    {
        private Action<byte[]> handler;
        private readonly UdpClient udpListener;
        private bool listening;

        public UdpAudioReceiver(IPEndPoint endPoint)
        {
            udpListener = new UdpClient();
            udpListener.ExclusiveAddressUse = false;
            udpListener.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            udpListener.Client.Bind(endPoint);

            ThreadPool.QueueUserWorkItem(ListenerThread, endPoint);
            listening = true;
        }

        private void ListenerThread(object state)
        {
            var endPoint = (IPEndPoint)state;
            try
            {
                while (listening)
                {
                    byte[] b = udpListener.Receive(ref endPoint);
                    handler?.Invoke(b);
                }
            }
            catch (SocketException)
            {
                // usually not a problem - just means we have disconnected
            }
        }

        public void Dispose()
        {
            listening = false;
            udpListener?.Close();
        }

        public void OnReceived(Action<byte[]> onAudioReceivedAction)
        {
            handler = onAudioReceivedAction;
        }
    }
}
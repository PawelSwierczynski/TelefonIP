using System;

namespace NAudioDemo.NetworkChatDemo
{
    public interface IAudioSender : IDisposable
    {
        void Send(byte[] payload);
    }
}
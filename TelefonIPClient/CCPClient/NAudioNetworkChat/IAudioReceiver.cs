using System;

namespace NAudioDemo.NetworkChatDemo
{
    public interface IAudioReceiver : IDisposable
    {
        void OnReceived(Action<byte[]> handler);
    }
}
using System;

namespace Noobie.Sanguosha.Infrastructure
{
    internal class DisposableSubscription<T> : IDisposable
    {
        private Action<T> m_Handler;
        private bool m_IsDisposed;
        private IMessageChannel<T> m_MessageChannel;

        public DisposableSubscription(IMessageChannel<T> messageChannel, Action<T> handler)
        {
            m_MessageChannel = messageChannel;
            m_Handler = handler;
        }
        
        public void Dispose()
        {
            if (!m_IsDisposed)
            {
                m_IsDisposed = true;

                if (!m_MessageChannel.IsDisposed)
                {
                    m_MessageChannel.Unsubscribe(m_Handler);
                }

                m_Handler = null;
                m_MessageChannel = null;
            }
        }
    }
}

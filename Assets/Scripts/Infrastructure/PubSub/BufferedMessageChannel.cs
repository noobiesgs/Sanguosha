using System;

namespace Noobie.Sanguosha.Infrastructure
{
    public class BufferedMessageChannel<T> : MessageChannel<T>, IBufferedMessageChannel<T>
    {
        public bool HasBufferedMessage { get; private set; }

        public T BufferedMessage { get; private set; }

        public override void Publish(T message)
        {
            HasBufferedMessage = true;
            BufferedMessage = message;
            base.Publish(message);
        }

        public override IDisposable Subscribe(Action<T> handler)
        {
            var subscription = base.Subscribe(handler);

            if (HasBufferedMessage)
            {
                handler?.Invoke(BufferedMessage);
            }

            return subscription;
        }
    }
}

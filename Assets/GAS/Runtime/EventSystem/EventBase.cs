using System;

namespace GAS.Runtime
{
    public class EventBase<T> where T : EventArgs
    {
        public event EventHandler<T> EventHandler;

        public void Publish(T args)
        {
            EventHandler?.Invoke(this, args);
        }

        public void Subscribe(EventHandler<T> handler)
        {
            EventHandler += handler;
        }

        public void Unsubscribe(EventHandler<T> handler)
        {
            EventHandler -= handler;
        }
    }
}
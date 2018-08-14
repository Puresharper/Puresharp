using System;

namespace Puresharp
{
    public class Listener<T> : IListener<T>
        where T : class
    {
        private Action<T> m_Action;

        public Listener(Action<T> action)
        {
            this.m_Action = action;
        }

        public void Listen(T item)
        {
            this.m_Action(item);
        }
    }
}

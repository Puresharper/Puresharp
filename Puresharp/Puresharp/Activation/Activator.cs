using System;

namespace Puresharp
{
    internal class Activator<T>
    {
        private Func<T> m_Activate;

        public Activator(T value)
        {
            this.m_Activate = new Func<T>(() => value);
        }

        public Func<T> Activate
        {
            get { return this.m_Activate; }
        }
    }
}

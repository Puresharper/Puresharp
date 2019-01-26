using System;
using System.Collections.Generic;

namespace Puresharp
{
    internal class Resolver
    {
        private Dictionary<Type, Func<Resolver, Reservation, object>> m_Dictionary;
        private Reservation m_Reservation;

        public Resolver(Dictionary<Type, Func<Resolver, Reservation, object>> dictionary)
        {
            this.m_Dictionary = dictionary;
            this.m_Reservation = new Reservation();
        }

        public T Resolve<T>()
            where T : class
        {
            return this.m_Dictionary[Metadata<T>.Type](this, this.m_Reservation) as T;
        }

        public void Dispose()
        {
            this.m_Reservation.Dispose();
        }
    }
}

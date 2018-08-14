using System;
using System.Collections.Generic;

namespace Puresharp
{
    public class Resolver : IResolver
    {
        private Dictionary<Type, Func<Resolver, Reservation, object>> m_Dictionary;
        private Reservation m_Reservation;

        public Resolver(Dictionary<Type, Func<Resolver, Reservation, object>> dictionary)
        {
            this.m_Dictionary = dictionary;
            this.m_Reservation = new Reservation();
        }

        T IResolver.Resolve<T>()
        {
            return this.m_Dictionary[Metadata<T>.Type](this, this.m_Reservation) as T;
        }

        public void Dispose()
        {
            this.m_Reservation.Dispose();
        }
    }
}

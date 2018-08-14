using System;

namespace Puresharp
{
    public partial class Container
    {
        internal class Map
        {
            private Type m_Type;
            private Func<Resolver, Reservation, object> m_Activate;
            private Instantiation m_Instantiation;

            public Map(Type type, Func<Resolver, Reservation, object> activate, Instantiation instantiation)
            {
                this.m_Type = type;
                this.m_Activate = activate;
                this.m_Instantiation = instantiation;
            }

            public Type Type
            {
                get { return this.m_Type; }
            }

            public Func<Resolver, Reservation, object> Activate
            {
                get { return this.m_Activate; }
            }

            public Instantiation Instantiation
            {
                get { return this.m_Instantiation; }
            }
        }
    }
}

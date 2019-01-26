using System;
using System.Linq.Expressions;

namespace Puresharp
{
    internal partial class Container
    {
        internal class Map
        {
            private Type m_Type;
            private Func<Resolver, Reservation, object> m_Activate;
            private LambdaExpression m_Activation;
            private Instantiation m_Instantiation;

            public Map(Type type, Func<Resolver, Reservation, object> activate, LambdaExpression activation, Instantiation instantiation)
            {
                this.m_Type = type;
                this.m_Activate = activate;
                this.m_Activation = activation;
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

            public LambdaExpression Activation
            {
                get { return this.m_Activation; }
            }

            public Instantiation Instantiation
            {
                get { return this.m_Instantiation; }
            }
        }
    }
}

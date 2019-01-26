using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Puresharp
{
    internal class Module<T> : IModule<T>
        where T : class
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Expression<Func<T>> m_Activation;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Instantiation m_Instantiation;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Resolver m_Resolver;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool m_Activated;

        public Module(Expression<Func<T>> activation, Instantiation instantiation, Resolver resolver)
        {
            this.m_Activation = activation;
            this.m_Instantiation = instantiation;
            this.m_Resolver = resolver;
            this.m_Activated = false;
        }

        public Expression<Func<T>> Activation
        {
            get { return this.m_Activation; }
        }

        public Instantiation Instantiation
        {
            get { return this.m_Instantiation; }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool Activated
        {
            get { return this.m_Activated; }
        }

        public T Value 
        {
            get
            {
                this.m_Activated = true;
                return this.m_Resolver.Resolve<T>();
            }
        }

        public void Dispose()
        {
            this.m_Resolver.Dispose();
        }
    }
}

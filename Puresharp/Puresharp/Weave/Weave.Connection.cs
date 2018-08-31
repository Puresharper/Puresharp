using System;
using System.Diagnostics;
using System.Reflection;

namespace Puresharp
{
    public partial class Weave 
    {
        public partial class Connection : IConnection
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Aspect m_Aspect;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private MethodBase m_Method;

            internal Connection(Aspect aspect, MethodBase method)
            {
                this.m_Aspect = aspect;
                this.m_Method = method;
            }

            public Aspect Aspect
            {
                get { return this.m_Aspect; }
            }

            public MethodBase Method
            {
                get { return this.m_Method; }
            }
        }
    }
}

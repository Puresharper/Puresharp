using System;
using System.Diagnostics;

namespace Puresharp
{
    public partial class Weave : IWeave
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Aspect m_Aspect;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Pointcut m_Pointcut;

        internal Weave(Aspect aspect, Pointcut pointcut)
        {
            this.m_Aspect = aspect;
            this.m_Pointcut = pointcut;
        }

        public Aspect Aspect
        {
            get { return this.m_Aspect; }
        }

        public Pointcut Pointcut
        {
            get { return this.m_Pointcut; }
        }
    }
}

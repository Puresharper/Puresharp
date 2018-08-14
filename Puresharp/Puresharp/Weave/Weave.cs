using System;
using System.Reflection;

namespace Puresharp
{
    internal class Weave : IWeave
    {
        private Aspect m_Aspect;
        private MethodBase m_Method;

        public Weave(Aspect aspect, MethodBase method)
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

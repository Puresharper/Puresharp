using System;
using System.Reflection;

namespace Puresharp.Underground
{
    public class Invocation
    {
        private MethodBase m_Method;
        private object m_Instance;
        private object[] m_Arguments;

        public Invocation(MethodBase method, object instance, object[] arguments)
        {
            this.m_Method = method;
            this.m_Instance = instance;
            this.m_Arguments = arguments;
        }

        public MethodBase Method
        {
            get { return this.m_Method; }
        }

        public object Instance
        {
            get { return this.m_Method; }
        }

        public object[] Arguments
        {
            get { return this.m_Arguments; }
        }
    }
}

using System;
using System.Reflection;

namespace Puresharp
{
    public partial class Advisor
    {
        public class Generator : Advisor.IGenerator
        {
            private MethodBase m_Method;
            
            internal Generator(MethodBase method)
            {
                this.m_Method = method;
            }

            MethodBase Advisor.IGenerator.Method
            {
                get { return this.m_Method; }
            }
        }
    }
}
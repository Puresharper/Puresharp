using System;
using System.Reflection;

namespace Puresharp
{
    public partial class Advisor
    {
        public partial class Parameter
        {
            public class Supervision
            {
                private MethodBase m_Method;
                private ParameterInfo m_Parameter;

                public Supervision(MethodBase method, ParameterInfo parameter)
                {
                    this.m_Method = method;
                    this.m_Parameter = parameter;
                }

                public MethodBase Method
                {
                    get { return this.m_Method; }
                }

                public ParameterInfo Parameter
                {
                    get { return this.m_Parameter; }
                }
            }
        }

        public partial class Parameter<T>
        {
            public class Supervision
            {
                private MethodBase m_Method;
                private ParameterInfo m_Parameter;
                private T m_Attribute;

                public Supervision(MethodBase method, ParameterInfo parameter, T attribute)
                {
                    this.m_Method = method;
                    this.m_Parameter = parameter;
                    this.m_Attribute = attribute;
                }

                public MethodBase Method
                {
                    get { return this.m_Method; }
                }

                public ParameterInfo Parameter
                {
                    get { return this.m_Parameter; }
                }

                public T Attribute
                {
                    get { return this.m_Attribute; }
                }
            }
        }
    }
}
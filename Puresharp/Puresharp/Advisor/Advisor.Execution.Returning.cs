using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Puresharp
{
    public partial class Advisor
    {
        public partial class Execution
        {
            public partial class Returning
            {
                private MethodBase m_Method;
                private Expression m_Instance;
                private Collection<Expression> m_Arguments;

                internal Returning(MethodBase method, Expression instance, Collection<Expression> arguments)
                {
                    this.m_Method = method;
                    this.m_Instance = instance;
                    this.m_Arguments = arguments;
                }

                public MethodBase Method
                {
                    get { return this.m_Method; }
                }

                public Expression Instance
                {
                    get { return this.m_Instance; }
                }

                public Collection<Expression> Arguments
                {
                    get { return this.m_Arguments; }
                }
            }
        }
    }
}

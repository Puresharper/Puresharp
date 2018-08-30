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
                public partial class Value
                {
                    private MethodBase m_Method;
                    private Expression m_Instance;
                    private Collection<Expression> m_Arguments;
                    private Expression m_Return;

                    internal Value(MethodBase method, Expression instance, Collection<Expression> arguments, Expression @return)
                    {
                        this.m_Method = method;
                        this.m_Instance = instance;
                        this.m_Arguments = arguments;
                        this.m_Return = @return;
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

                    public Expression Return
                    {
                        get { return this.m_Return; }
                    }
                }
            }
        }
    }
}
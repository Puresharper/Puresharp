using System;
using System.Linq.Expressions;

namespace Puresharp
{
    public partial class Advisor
    {
        public partial class Execution
        {
            public partial class Throwing
            {
                private Advisor.Invocation m_Invocation;
                private Expression m_Exception;

                public Throwing(Advisor.Invocation invocation, Expression exception)
                {
                    this.m_Invocation = invocation;
                    this.m_Exception = exception;
                }

                public Advisor.Invocation Invocation
                {
                    get { return this.m_Invocation; }
                }

                public Expression Exception
                {
                    get { return this.m_Exception; }
                }
            }
        }
    }
}

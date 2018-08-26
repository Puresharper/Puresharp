using System;
using System.Linq.Expressions;

namespace Puresharp
{
    public partial class Advisor
    {
        public partial class Execution
        {
            public class Returning
            {
                private Advisor.Invocation m_Invocation;
                private Expression m_Return;

                public Returning(Advisor.Invocation invocation, Expression @return)
                {
                    this.m_Invocation = invocation;
                    this.m_Return = @return;
                }

                public Advisor.Invocation Invocation
                {
                    get { return this.m_Invocation; }
                }

                public Expression Return
                {
                    get { return this.m_Return; }
                }
            }
        }
    }
}

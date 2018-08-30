using System;

namespace Puresharp
{
    public partial class Advisor
    {
        public partial class After
        {
            public partial class Returning : Advisor.After.IReturning
            {
                private Advisor.IAfter m_After;

                internal Returning(Advisor.IAfter after)
                {
                    this.m_After = after;
                }

                Advisor.IAfter Advisor.After.IReturning.After
                {
                    get { return this.m_After; }
                }
            }
        }
    }
}

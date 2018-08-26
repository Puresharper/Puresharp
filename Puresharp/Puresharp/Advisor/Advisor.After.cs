using System;

namespace Puresharp
{
    public partial class Advisor
    {
        public partial class After : Advisor.IAfter
        {
            private Advisor.IGenerator m_Generator;

            internal After(Advisor.IGenerator generator)
            {
                this.m_Generator = generator;
            }

            Advisor.IGenerator Advisor.IAfter.Generator
            {
                get { return this.m_Generator; }
            }
        }
    }
}

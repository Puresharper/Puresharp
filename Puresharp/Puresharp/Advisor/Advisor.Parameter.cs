using System;

namespace Puresharp
{
    public partial class Advisor
    {
        public class Parameter : Advisor.IParameter
        {
            private Advisor.IGenerator m_Generator;

            internal Parameter(Advisor.IGenerator generator)
            {
                this.m_Generator = generator;
            }

            Advisor.IGenerator Advisor.IParameter.Generator
            {
                get { return this.m_Generator; }
            }
        }

        public class Parameter<T> : Advisor.IParameter<T>
            where T : Attribute
        {
            private Advisor.IGenerator m_Generator;

            internal Parameter(Advisor.IGenerator generator)
            {
                this.m_Generator = generator;
            }

            Advisor.IGenerator Advisor.IParameter<T>.Generator
            {
                get { return this.m_Generator; }
            }
        }
    }
}
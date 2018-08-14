using System;
using System.Linq.Expressions;

namespace Puresharp
{
    internal class Setup<T> : ISetup<T>
            where T : class
    {
        private Expression<Func<T>> m_Activation;
        private Instantiation m_Instantiation;

        public Setup(Expression<Func<T>> activation, Instantiation instantiation)
        {
            this.m_Activation = activation;
            this.m_Instantiation = instantiation;
        }

        public Expression<Func<T>> Activation
        {
            get { return this.m_Activation; }
            set { this.m_Activation = value; }
        }

        LambdaExpression ISetup.Activation
        {
            get { return this.m_Activation; }
        }

        public Instantiation Instantiation
        {
            get { return this.m_Instantiation; }
            set { this.m_Instantiation = value; }
        }

        void IVisitable.Accept(IVisitor visitor)
        {
            visitor.Visit<T>();
        }
    }
}

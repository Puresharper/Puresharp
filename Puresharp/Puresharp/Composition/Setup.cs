using System;
using System.Linq.Expressions;

namespace Puresharp
{
    internal abstract class Setup
    {
        abstract public void Accept(Composition.IVisitor visitor);
    }

    internal class Setup<T> : Setup, ISetup<T>
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

        public Instantiation Instantiation
        {
            get { return this.m_Instantiation; }
            set { this.m_Instantiation = value; }
        }

        public override void Accept(Composition.IVisitor visitor)
        {
            visitor.Visit<T>(this);
        }
    }
}

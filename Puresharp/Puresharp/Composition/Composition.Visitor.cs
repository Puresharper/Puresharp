using System;
using System.Linq.Expressions;

namespace Puresharp
{
    public sealed partial class Composition
    {
        internal class Visitor : Composition.IVisitor
        {
            private Composition.IVisitor m_Visitor;

            public Visitor(Composition.IVisitor visitor)
            {
                this.m_Visitor = visitor;
            }

            public void Visit<T>(ISetup<T> setup)
                where T : class
            {
                if (setup.Activation == null) { return; }
                this.m_Visitor.Visit<T>(setup);
            }
        }
    }
}

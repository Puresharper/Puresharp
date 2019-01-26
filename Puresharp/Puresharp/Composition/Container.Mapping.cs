using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Puresharp
{
    internal partial class Container
    {
        private class Mapping : Composition.IVisitor, IEnumerable<Map>, IDisposable
        {
            private Composition m_Composition;
            private LinkedList<Map> m_Value = new LinkedList<Map>();

            public Mapping(Composition composition)
            {
                this.m_Composition = composition;
                this.m_Composition.Accept(this);
            }

            void Composition.IVisitor.Visit<T>(ISetup<T> setup)
            {
                var _body = Expression.Convert(new Converter(Parameter<Resolver>.Expression).Visit(setup.Activation.Body), Metadata<object>.Type);
                var _activate = Expression.Lambda<Func<Resolver, Reservation, object>>(_body, Parameter<Resolver>.Expression, Parameter<Reservation>.Expression).Compile();
                this.m_Value.AddLast(new Map(Metadata<T>.Type, Proxy<T>.Create(_activate), setup.Activation, setup.Instantiation));
            }
            
            public void Dispose()
            {
                this.m_Value = null;
                this.m_Composition = null;
            }

            IEnumerator<Map> IEnumerable<Map>.GetEnumerator()
            {
                return this.m_Value.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.m_Value.GetEnumerator();
            }
        }
    }
}

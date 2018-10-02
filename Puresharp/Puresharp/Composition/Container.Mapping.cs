using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Puresharp
{
    public partial class Container
    {
        private class Mapping : IVisitor, IEnumerable<Map>, IDisposable
        {
            private IComposition m_Composition;
            private LinkedList<Map> m_Value = new LinkedList<Map>();

            public Mapping(IComposition composition)
            {
                this.m_Composition = composition;
                this.m_Composition.Accept(this);
            }

            void IVisitor.Visit<T>(Func<T> value)
            {
                var _setup = this.m_Composition.Setup<T>();
                var _body = Expression.Convert(new Converter(Parameter<Resolver>.Expression).Visit(_setup.Activation.Body), Metadata<object>.Type);
                var _activate = Expression.Lambda<Func<Resolver, Reservation, object>>(_body, Parameter<Resolver>.Expression, Parameter<Reservation>.Expression).Compile();
                this.m_Value.AddLast(new Map(Metadata<T>.Type, Proxy<T>.Create(_activate), _setup.Instantiation));
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

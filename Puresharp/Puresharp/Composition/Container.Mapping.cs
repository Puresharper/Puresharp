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
            private IComposition m_Setting;
            private LinkedList<Map> m_Value = new LinkedList<Map>();

            public Mapping(IComposition setting)
            {
                this.m_Setting = setting;
                this.m_Setting.Accept(this);
            }

            void IVisitor.Visit<T>()
            {
                var _setup = this.m_Setting.Setup<T>();
                var _body = Expression.Convert(new Converter(Parameter<Resolver>.Expression).Visit(_setup.Activation.Body), Metadata<object>.Type);
                var _activate = Expression.Lambda<Func<Resolver, Reservation, object>>(_body, Parameter<Resolver>.Expression, Parameter<Reservation>.Expression).Compile();
                this.m_Value.AddLast(new Map(Metadata<T>.Type, Proxy<T>.Create(_activate), _setup.Instantiation));
            }
            
            public void Dispose()
            {
                this.m_Value = null;
                this.m_Setting = null;
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

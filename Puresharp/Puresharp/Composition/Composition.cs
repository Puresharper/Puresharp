using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Puresharp
{
    /// <summary>
    /// Composition.
    /// </summary>
    public sealed class Composition : IComposition
    {
        private Dictionary<Type, ISetup> m_Dictionary;

        /// <summary>
        /// Create a composition.
        /// </summary>
        public Composition()
        {
            this.m_Dictionary = new Dictionary<Type, ISetup>();
        }

        void IVisitable.Accept(IVisitor visitor)
        {
            foreach (var _item in this.m_Dictionary)
            {
                if (_item.Value.Activation == null) { continue; }
                _item.Value.Accept(visitor);
            }
        }

        /// <summary>
        /// Try get setup for specific module.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ISetup<T> Setup<T>()
            where T : class
        {
            this.m_Dictionary.TryGetValue(Metadata<T>.Type, out var _value);
            return _value as ISetup<T>;
        }

        /// <summary>
        /// Setup specific module by specifying how to create implementation of module.
        /// </summary>
        /// <typeparam name="T">Type of module</typeparam>
        /// <param name="activation">Way to create implementation</param>
        /// <param name="instantiation">Type of instantiation to setup</param>
        /// <returns>Setup</returns>
        public ISetup<T> Setup<T>(Expression<Func<T>> activation, Instantiation instantiation)
            where T : class
        {
            if (this.m_Dictionary.TryGetValue(Metadata<T>.Type, out var _value)) { return _value as ISetup<T>; }
            _value = new Setup<T>(activation, instantiation);
            this.m_Dictionary.Add(Metadata<T>.Type, _value);
            return _value as ISetup<T>;
        }
    }
}

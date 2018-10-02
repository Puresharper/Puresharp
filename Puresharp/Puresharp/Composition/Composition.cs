using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    /// <summary>
    /// Composition.
    /// </summary>
    public sealed partial class Composition : IComposition
    {
        internal const string Assembly = "Puresharp.Composition.Proxy.Assembly, PublicKey=002400000480000094000000060200000024000052534131000400000100010051A76D0BF5695EB709657A23340D31BF2831DBAEF43A4929F442F960875159CCAD93FBC5994577761C35CFFBA0AEF27255D462A61E1D23D45CF06D9C23FABB59CAB1C6FE510C653CD5843CBC911DBABB0E29201DE8C6035CDEDD3ABEEDBC081C5F85E51D84D6CB068DCF8E9682B2AC3FEE59179221C3E1618A8C3275A8EEDECA";
        static private ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule(Composition.Assembly.Substring(0, Composition.Assembly.IndexOf(',')), Composition.Assembly.Substring(Composition.Assembly.IndexOf('=') + 1));
        static internal ModuleBuilder Module { get { return Composition.m_Module; } }

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
        {
            if (!Metadata<T>.Type.IsInterface) { throw new NotSupportedException(); }
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
        {
            if (!Metadata<T>.Type.IsInterface) { throw new NotSupportedException(); }
            if (this.m_Dictionary.TryGetValue(Metadata<T>.Type, out var _value)) { return _value as ISetup<T>; }
            _value = new Setup<T>(activation, instantiation);
            this.m_Dictionary.Add(Metadata<T>.Type, _value);
            return _value as ISetup<T>;
        }
    }
}

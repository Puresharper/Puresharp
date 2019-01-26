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
    public sealed partial class Composition
    {
        internal const string Assembly = "Puresharp.Composition.Proxy.Assembly, PublicKey=002400000480000094000000060200000024000052534131000400000100010051A76D0BF5695EB709657A23340D31BF2831DBAEF43A4929F442F960875159CCAD93FBC5994577761C35CFFBA0AEF27255D462A61E1D23D45CF06D9C23FABB59CAB1C6FE510C653CD5843CBC911DBABB0E29201DE8C6035CDEDD3ABEEDBC081C5F85E51D84D6CB068DCF8E9682B2AC3FEE59179221C3E1618A8C3275A8EEDECA";
        static private ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule(Composition.Assembly.Substring(0, Composition.Assembly.IndexOf(',')), Composition.Assembly.Substring(Composition.Assembly.IndexOf('=') + 1));
        static internal ModuleBuilder Module { get { return Composition.m_Module; } }

        private Dictionary<Type, Setup> m_Dictionary;

        /// <summary>
        /// Create a composition.
        /// </summary>
        public Composition()
        {
            this.m_Dictionary = new Dictionary<Type, Setup>();
        }

        public Composition Accept(IVisitor visitor)
        {
            var _visitor = new Composition.Visitor(visitor);
            foreach (var _item in this.m_Dictionary) { _item.Value.Accept(_visitor); }
            return this;
        }

        /// <summary>
        /// Try get setup for specific module.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Setup</returns>
        public ISetup<T> Setup<T>()
            where T : class
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
        /// <returns>This</returns>
        public Composition Setup<T>(Expression<Func<T>> activation, Instantiation instantiation)
            where T : class
        {
            if (!Metadata<T>.Type.IsInterface) { throw new NotSupportedException(); }
            if (this.m_Dictionary.TryGetValue(Metadata<T>.Type, out var _value))
            {
                (_value as ISetup<T>).Activation = activation;
                (_value as ISetup<T>).Instantiation = instantiation;
                return this;
            }
            _value = new Setup<T>(activation, instantiation);
            this.m_Dictionary.Add(Metadata<T>.Type, _value);
            return this;
        }

        /// <summary>
        /// Setup specific multiton module by specifying how to create implementation of module.
        /// </summary>
        /// <typeparam name="T">Type of module</typeparam>
        /// <param name="activation">Way to create implementation</param>
        /// <returns>This</returns>
        public Composition Setup<T>(Expression<Func<T>> activation)
            where T : class
        {
            return this.Setup<T>(activation, Instantiation.Multiton);
        }

        /// <summary>
        /// Include a fallback composition.
        /// </summary>
        /// <param name="composition">Fallback</param>
        /// <returns>Composition</returns>
        public Composition Then(Composition composition)
        {
            if (composition == null || composition == this) { return this; }
            composition.Accept(new Fallback(this, composition));
            return this;
        }

        /// <summary>
        /// Create a container based on this composition.
        /// </summary>
        /// <returns>Container</returns>
        public IContainer Materialize()
        {
            return new Container(this);
        }

        private class Fallback : Composition.IVisitor
        {
            static private Expression<Func<T>> Combine<T>(Expression<Func<T>> authorithy, Expression<Func<T>> auxiliary)
                where T : class
            {
                var _instance = Expression.Parameter(Metadata<T>.Type);
                return Expression.Lambda<Func<T>>
                (
                    Expression.Block
                    (
                        new ParameterExpression[] { _instance },
                        Expression.Assign(_instance, authorithy.Body),
                        Expression.IfThen
                        (
                            Expression.Call
                            (
                                Metadata.Method(() => object.ReferenceEquals(Metadata<object>.Value, Metadata<object>.Value)), 
                                Expression.TypeAs(_instance, Metadata<object>.Type), 
                                Expression.Constant(null, Metadata<object>.Type))
                            ,
                            Expression.Assign(_instance, auxiliary.Body)
                        ),
                        _instance
                    )
                );
            }

            private Composition m_Authority;
            private Composition m_Auxiliary;

            public Fallback(Composition authorithy, Composition auxiliary)
            {
                this.m_Authority = authorithy;
                this.m_Auxiliary = auxiliary;
            }

            public void Visit<T>(ISetup<T> setup)
                where T : class
            {
                var _setup = this.m_Authority.Setup<T>();
                this.m_Authority.Setup<T>(_setup == null ? setup.Activation : Fallback.Combine(_setup.Activation, setup.Activation), setup.Instantiation);
            }
        }
    }
}

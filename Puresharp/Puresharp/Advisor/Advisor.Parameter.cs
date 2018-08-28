using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    public partial class Advisor
    {
        public partial class Parameter : Advisor.IParameter
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

            public Advisor Visit<T>()
                    where T : class, IVisitor, new()
            {
                return this.Visit(_Parameter => Singleton<T>.Value);
            }

            public Advisor Visit(IVisitor visitor)
            {
                return this.Visit(_Parameter => visitor);
            }

            public Advisor Visit(Func<IVisitor> visitor)
            {
                return this.Visit(_Parameter => visitor());
            }

            public Advisor Visit(Func<ParameterInfo, IVisitor> visitor)
            {
                var _signature = this.m_Generator.Method.GetParameters();
                if (!_signature.Any()) { return this.m_Generator.Around(Advisor.Null); }
                var _type = Aspect.Module.DefineType(Guid.NewGuid().ToString("N"), TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Serializable, Metadata<Advice>.Type, new Type[] { Metadata<IAdvice>.Type });
                var _field = _type.DefineField("<Index>", Metadata<int>.Type, FieldAttributes.Private);
                var _method = _type.DefineMethod("IAdvice.Argument`1", MethodAttributes.Private | MethodAttributes.Virtual, CallingConventions.HasThis);
                var _genericity = _method.DefineGenericParameters("T")[0];
                _method.SetParameters(_genericity.MakeByRefType());
                var _body = _method.GetILGenerator();
                var _table = new Label[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++) { _table[_index] = _body.DefineLabel(); }
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldfld, _field);
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldfld, _field);
                _body.Emit(OpCodes.Ldc_I4_1);
                _body.Emit(OpCodes.Add);
                _body.Emit(OpCodes.Stfld, _field);
                _body.Emit(OpCodes.Switch, _table);
                _body.Emit(OpCodes.Ret);
                foreach (var _parameter in _signature)
                {
                    _body.MarkLabel(_table[_parameter.Position]);
                    _body.Emit(OpCodes.Ldsfld, _type.DefineField($"<{ _parameter.Name }>", Metadata<IVisitor>.Type, FieldAttributes.Static | FieldAttributes.Private));
                    _body.Emit(OpCodes.Ldarg_1);
                    _body.Emit(OpCodes.Ldobj, _genericity);
                    _body.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(Metadata<Activator<object>>.Type.GetGenericTypeDefinition().MakeGenericType(_genericity), Metadata<Activator<object>>.Type.GetGenericTypeDefinition().GetConstructors().Single()));
                    _body.Emit(OpCodes.Call, TypeBuilder.GetMethod(Metadata<Activator<object>>.Type.GetGenericTypeDefinition().MakeGenericType(_genericity), Metadata<Activator<object>>.Type.GetGenericTypeDefinition().GetProperty(Metadata<Activator<object>>.Property(_Activator => _Activator.Activate).Name).GetGetMethod(true)));
                    _body.Emit(OpCodes.Callvirt, Metadata<IVisitor>.Method(_Visitor => _Visitor.Visit(Metadata<Func<object>>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_genericity));
                    _body.Emit(OpCodes.Ret);
                }
                _type.DefineMethodOverride(_method, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Argument(ref Metadata<object>.Value)).GetGenericMethodDefinition());
                var _validation = _type.CreateType();
                foreach (var _parameter in _signature)
                {
                    _validation.GetField($"<{ _parameter.Name }>", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, visitor(_parameter));
                }
                return this.m_Generator.Around(Expression.Lambda<Func<IAdvice>>(Expression.New(_validation)).Compile());
            }
        }

        public partial class Parameter<T> : Advisor.IParameter<T>
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

            public Advisor Visit<T>()
                where T : class, IVisitor, new()
            {
                return this.Visit((_Parameter, _Attribute) => Singleton<T>.Value);
            }

            public Advisor Visit(IVisitor visitor)
            {
                return this.Visit((_Parameter, _Attribute) => visitor);
            }

            public Advisor Visit(Func<IVisitor> visitor)
            {
                return this.Visit((_Parameter, _Attribute) => visitor());
            }

            public Advisor Visit(Func<ParameterInfo, IVisitor> visitor)
            {
                return this.Visit((_Parameter, _Attribute) => visitor(_Parameter));
            }

            public Advisor Visit(Func<ParameterInfo, T, IVisitor> visitor)
            {
                var _signature = this.m_Generator.Method.GetParameters();
                if (!_signature.Any(_Parameter => Attribute<T>.On(this.m_Generator.Method, _Parameter))) { return this.m_Generator.Around(Advisor.Null); }
                var _type = Aspect.Module.DefineType(Guid.NewGuid().ToString("N"), TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Serializable, Metadata<Advice>.Type, new Type[] { Metadata<IAdvice>.Type });
                var _field = _type.DefineField("<Index>", Metadata<int>.Type, FieldAttributes.Private);
                var _method = _type.DefineMethod("IAdvice.Argument`1", MethodAttributes.Private | MethodAttributes.Virtual, CallingConventions.HasThis);
                var _genericity = _method.DefineGenericParameters("T")[0];
                _method.SetParameters(_genericity.MakeByRefType());
                var _body = _method.GetILGenerator();
                var _table = new Label[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++) { _table[_index] = _body.DefineLabel(); }
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldfld, _field);
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldfld, _field);
                _body.Emit(OpCodes.Ldc_I4_1);
                _body.Emit(OpCodes.Add);
                _body.Emit(OpCodes.Stfld, _field);
                _body.Emit(OpCodes.Switch, _table);
                _body.Emit(OpCodes.Ret);
                foreach (var _parameter in _signature)
                {
                    _body.MarkLabel(_table[_parameter.Position]);
                    if (Attribute<T>.On(this.m_Generator.Method, _parameter))
                    {
                        _body.Emit(OpCodes.Ldsfld, _type.DefineField($"<{ _parameter.Name }>", Metadata<IVisitor>.Type, FieldAttributes.Static | FieldAttributes.Private));
                        _body.Emit(OpCodes.Ldarg_1);
                        _body.Emit(OpCodes.Ldobj, _genericity);
                        _body.Emit(OpCodes.Newobj, TypeBuilder.GetConstructor(Metadata<Activator<object>>.Type.GetGenericTypeDefinition().MakeGenericType(_genericity), Metadata<Activator<object>>.Type.GetGenericTypeDefinition().GetConstructors().Single()));
                        _body.Emit(OpCodes.Call, TypeBuilder.GetMethod(Metadata<Activator<object>>.Type.GetGenericTypeDefinition().MakeGenericType(_genericity), Metadata<Activator<object>>.Type.GetGenericTypeDefinition().GetProperty(Metadata<Activator<object>>.Property(_Activator => _Activator.Activate).Name).GetGetMethod(true)));
                        _body.Emit(OpCodes.Callvirt, Metadata<IVisitor>.Method(_Visitor => _Visitor.Visit(Metadata<Func<object>>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_genericity));
                    }
                    _body.Emit(OpCodes.Ret);
                }
                _type.DefineMethodOverride(_method, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Argument(ref Metadata<object>.Value)).GetGenericMethodDefinition());
                var _validation = _type.CreateType();
                foreach (var _parameter in _signature)
                {
                    if (Attribute<T>.On(this.m_Generator.Method, _parameter))
                    {
                        _validation.GetField($"<{ _parameter.Name }>", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, visitor(_parameter, Attribute<T>.From(this.m_Generator.Method, _parameter)));
                    }
                }
                return this.m_Generator.Around(Expression.Lambda<Func<IAdvice>>(Expression.New(_validation)).Compile());
            }
        }
    }
}
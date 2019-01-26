using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    internal partial class Container
    {
        static internal class Proxy<T>
        {
            static internal Func<Func<Resolver, Reservation, object>, Func<Resolver, Reservation, object>> Create = Proxy<T>.Compile();

            static private Func<Func<Resolver, Reservation, object>, Func<Resolver, Reservation, object>> Compile()
            {
                var _type = Composition.Module.DefineType($"<{ Declaration<T>.Value }>", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable, Metadata<object>.Type, new Type[] { Metadata<T>.Type });
                var _field = _type.DefineField("m_Value", Metadata<T>.Type, FieldAttributes.Private);
                _field.SetCustomAttribute(new CustomAttributeBuilder(Metadata.Constructor(() => new DebuggerBrowsableAttribute(Metadata<DebuggerBrowsableState>.Value)), new object[] { DebuggerBrowsableState.Never }));
                var _activate = _type.DefineField("m_Activate", Metadata<Func<object>>.Type, FieldAttributes.Private);
                _activate.SetCustomAttribute(new CustomAttributeBuilder(Metadata.Constructor(() => new DebuggerBrowsableAttribute(Metadata<DebuggerBrowsableState>.Value)), new object[] { DebuggerBrowsableState.Never }));
                var _constructor = _type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] { Metadata<Func<object>>.Type });
                var _body = _constructor.GetILGenerator();
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Call, Metadata<object>.Type.GetConstructors().Single());
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldarg_1);
                _body.Emit(OpCodes.Stfld, _activate);
                _body.Emit(OpCodes.Ret);
                var _property = _type.DefineProperty("Value", PropertyAttributes.HasDefault, _field.FieldType, Type.EmptyTypes);
                var _getter = _type.DefineMethod("get_Value", MethodAttributes.Public | MethodAttributes.SpecialName  | MethodAttributes.HideBySig, _field.FieldType, Type.EmptyTypes);
                _body = _getter.GetILGenerator();
                var _activated = _body.DefineLabel();
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldfld, _activate);
                _body.Emit(OpCodes.Brfalse, _activated);
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldfld, _activate);
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldnull);
                _body.Emit(OpCodes.Stfld, _activate);
                _body.Emit(OpCodes.Call, Metadata<Func<object>>.Method(_Function => _Function.Invoke()));
                _body.Emit(OpCodes.Stfld, _field);
                _body.MarkLabel(_activated);
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldfld, _field);
                _body.Emit(OpCodes.Ret);
                _property.SetGetMethod(_getter);
                _property.SetCustomAttribute(new CustomAttributeBuilder(Metadata.Constructor(() => new DebuggerBrowsableAttribute(Metadata<DebuggerBrowsableState>.Value)), new object[] { DebuggerBrowsableState.RootHidden }));
                _type.SetCustomAttribute(new CustomAttributeBuilder(Metadata.Constructor(() => new DebuggerDisplayAttribute(Metadata<string>.Value)), new object[] { "{ this.Value, nq }" }));
                foreach (var _method in new Type[] { Metadata<T>.Type }.Concat(Metadata<T>.Type.GetInterfaces()).SelectMany(_Type => _Type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))) { Proxy<T>.Compile(_type, _field, _activate,  _method); }
                if (!Metadata<IDisposable>.Type.IsAssignableFrom(Metadata<T>.Type))
                {
                    var _method = _type.DefineMethod("IDisposable.Dispose", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual);
                    _body = _method.GetILGenerator();
                    var _return = _body.DefineLabel();
                    var _clean = _body.DefineLabel();
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldfld, _activate);
                    _body.Emit(OpCodes.Brtrue, _return);
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldfld, _field);
                    _body.Emit(OpCodes.Isinst, Metadata<IDisposable>.Type);
                    _body.Emit(OpCodes.Brfalse, _return);
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldfld, _field);
                    _body.Emit(OpCodes.Callvirt, Metadata<IDisposable>.Type.GetMethods().Single());
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldnull);
                    _body.Emit(OpCodes.Stfld, _field);
                    _body.MarkLabel(_clean);
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldnull);
                    _body.Emit(OpCodes.Stfld, _activate);
                    _body.MarkLabel(_return);
                    _body.Emit(OpCodes.Ret);
                }
                var _activator = Expression.Lambda<Func<Func<object>, object>>(Expression.TypeAs(Expression.New(_type.CreateType().GetConstructors().Single(), Parameter<Func<object>>.Expression), Metadata<object>.Type), Parameter<Func<object>>.Expression).Compile();
                return new Func<Func<Resolver, Reservation, object>, Func<Resolver, Reservation, object>>(_Activate => new Func<Resolver, Reservation, object>((_Resolver, _Reservation) => _activator(() => _Activate(_Resolver, _Reservation))));
            }

            static private void Compile(TypeBuilder type, FieldBuilder value, FieldBuilder activate, MethodInfo method)
            {
                var _parameters = method.GetParameters();
                var _method = type.DefineMethod(method.Name, method.Attributes & ~MethodAttributes.Abstract, method.CallingConvention);
                var _body = _method.GetILGenerator();
                if (method.IsGenericMethod)
                {
                    var _dictionary = method.GetGenericArguments().ToDictionary((genericTypeParameter) => genericTypeParameter.Name);
                    var _genericity = _method.DefineGenericParameters(_dictionary.Keys.ToArray()).ToDictionary((genericTypeParameterBuilder) => genericTypeParameterBuilder.Name);
                    var _resolve = new Func<Type, Type>(_Type => _Type.IsGenericParameter ? _genericity[_Type.Name] : _Type);
                    foreach (var _name in _dictionary.Keys)
                    {
                        var _argument = _dictionary[_name];
                        var _generic = _genericity[_name];
                        _generic.SetBaseTypeConstraint(_resolve(_argument.BaseType));
                        _generic.SetInterfaceConstraints(_argument.GetInterfaces().Select(_Type => _resolve(_Type)).ToArray());
                        _generic.SetGenericParameterAttributes(_argument.GenericParameterAttributes);
                    }
                    _method.SetReturnType(_resolve(method.ReturnType));
                    _method.SetParameters(_parameters.Select(parameter => _resolve(parameter.ParameterType)).ToArray());
                    var _activated = _body.DefineLabel();
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldfld, activate);
                    _body.Emit(OpCodes.Brfalse, _activated);
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldfld, activate);
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldnull);
                    _body.Emit(OpCodes.Stfld, activate);
                    _body.Emit(OpCodes.Call, Metadata<Func<object>>.Method(_Function => _Function.Invoke()));
                    _body.Emit(OpCodes.Stfld, value);
                    _body.MarkLabel(_activated);
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldfld, value);
                    foreach (var parameter in _parameters)
                    {
                        _method.DefineParameter(parameter.Position, parameter.Attributes, parameter.Name);
                        _body.Emit(OpCodes.Ldarg, parameter.Position + 1);
                    }
                    _body.Emit(OpCodes.Callvirt, method.MakeGenericMethod(_genericity.Values.ToArray()));
                }
                else
                {
                    _method.SetReturnType(method.ReturnType);
                    _method.SetParameters(_parameters.Select(parameter => parameter.ParameterType).ToArray());
                    var _activated = _body.DefineLabel();
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldfld, activate);
                    _body.Emit(OpCodes.Brfalse, _activated);
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldfld, activate);
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldnull);
                    _body.Emit(OpCodes.Stfld, activate);
                    _body.Emit(OpCodes.Call, Metadata<Func<object>>.Method(_Function => _Function.Invoke()));
                    _body.Emit(OpCodes.Stfld, value);
                    _body.MarkLabel(_activated);
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldfld, value);
                    foreach (var parameter in _parameters) { _body.Emit(OpCodes.Ldarg, parameter.Position + 1); }
                    _body.Emit(OpCodes.Callvirt, method);
                }
                _body.Emit(OpCodes.Ret);
                type.DefineMethodOverride(_method, method);
            }
        }
    }
}

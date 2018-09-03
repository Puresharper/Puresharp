using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp.Underground
{
    static public class __Advisor
    {
        static private ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString("N")), AssemblyBuilderAccess.RunAndCollect).DefineDynamicModule(Guid.NewGuid().ToString("N"));

        static public Advisor Before(this Advisor.IGenerator @this, Action<object, object[]> advice)
        {
            var _type = __Advisor.m_Module.DefineType(Guid.NewGuid().ToString("N"), TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.Public);
            _type.DefineField(Guid.NewGuid().ToString("N"), Metadata<Action<object, object[]>>.Type, FieldAttributes.Static | FieldAttributes.Public);
            var _field = _type.CreateType().GetFields().Single();
            var _signature = @this.Method.GetParameters();
            if (@this.Method.IsStatic)
            {
                return @this.Before(_ILGenerator =>
                {
                    _ILGenerator.Emit(OpCodes.Ldsfld, _field);
                    _ILGenerator.Emit(OpCodes.Ldnull);
                    _ILGenerator.Emit(OpCodes.Ldc_I4, _signature.Length);
                    _ILGenerator.Emit(OpCodes.Newarr, Metadata<object>.Type);
                    foreach (var _parameter in _signature)
                    {
                        _ILGenerator.Emit(OpCodes.Dup);
                        _ILGenerator.Emit(OpCodes.Ldc_I4, _parameter.Position);
                        _ILGenerator.Emit(OpCodes.Ldarg, _parameter.Position);
                        if (_parameter.ParameterType.IsValueType) { _ILGenerator.Emit(OpCodes.Box, _parameter.ParameterType); }
                        _ILGenerator.Emit(OpCodes.Stelem_Ref);
                    }
                    _ILGenerator.Emit(OpCodes.Callvirt, Metadata<Action<object, object[]>>.Method(_Action => _Action.Invoke(Metadata<object>.Value, Metadata<object[]>.Value)));
                });
            }
            else
            {
                return @this.Before(_ILGenerator =>
                {
                    _ILGenerator.Emit(OpCodes.Ldsfld, _field);
                    _ILGenerator.Emit(OpCodes.Ldarg_0);
                    _ILGenerator.Emit(OpCodes.Ldc_I4, _signature.Length);
                    _ILGenerator.Emit(OpCodes.Newarr, Metadata<object>.Type);
                    foreach (var _parameter in _signature)
                    {
                        _ILGenerator.Emit(OpCodes.Dup);
                        _ILGenerator.Emit(OpCodes.Ldc_I4, _parameter.Position + 1);
                        _ILGenerator.Emit(OpCodes.Ldarg, _parameter.Position);
                        if (_parameter.ParameterType.IsValueType) { _ILGenerator.Emit(OpCodes.Box, _parameter.ParameterType); }
                        _ILGenerator.Emit(OpCodes.Stelem_Ref);
                    }
                    _ILGenerator.Emit(OpCodes.Callvirt, Metadata<Action<object, object[]>>.Method(_Action => _Action.Invoke(Metadata<object>.Value, Metadata<object[]>.Value)));
                });
            }
        }

        static public Advisor After(this Advisor.IGenerator @this, Action<object, object[]> advice)
        {
            var _type = __Advisor.m_Module.DefineType(Guid.NewGuid().ToString("N"), TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.Public);
            _type.DefineField(Guid.NewGuid().ToString("N"), Metadata<Action<object, object[]>>.Type, FieldAttributes.Static | FieldAttributes.Public);
            var _field = _type.CreateType().GetFields().Single();
            var _signature = @this.Method.GetParameters();
            if (@this.Method.IsStatic)
            {
                return @this.After(_ILGenerator =>
                {
                    _ILGenerator.Emit(OpCodes.Ldsfld, _field);
                    _ILGenerator.Emit(OpCodes.Ldnull);
                    _ILGenerator.Emit(OpCodes.Ldc_I4, _signature.Length);
                    _ILGenerator.Emit(OpCodes.Newarr, Metadata<object>.Type);
                    foreach (var _parameter in _signature)
                    {
                        _ILGenerator.Emit(OpCodes.Dup);
                        _ILGenerator.Emit(OpCodes.Ldc_I4, _parameter.Position);
                        _ILGenerator.Emit(OpCodes.Ldarg, _parameter.Position);
                        if (_parameter.ParameterType.IsValueType) { _ILGenerator.Emit(OpCodes.Box, _parameter.ParameterType); }
                        _ILGenerator.Emit(OpCodes.Stelem_Ref);
                    }
                    _ILGenerator.Emit(OpCodes.Callvirt, Metadata<Action<object, object[]>>.Method(_Action => _Action.Invoke(Metadata<object>.Value, Metadata<object[]>.Value)));
                });
            }
            else
            {
                return @this.After(_ILGenerator =>
                {
                    _ILGenerator.Emit(OpCodes.Ldsfld, _field);
                    _ILGenerator.Emit(OpCodes.Ldarg_0);
                    _ILGenerator.Emit(OpCodes.Ldc_I4, _signature.Length);
                    _ILGenerator.Emit(OpCodes.Newarr, Metadata<object>.Type);
                    foreach (var _parameter in _signature)
                    {
                        _ILGenerator.Emit(OpCodes.Dup);
                        _ILGenerator.Emit(OpCodes.Ldc_I4, _parameter.Position + 1);
                        _ILGenerator.Emit(OpCodes.Ldarg, _parameter.Position);
                        if (_parameter.ParameterType.IsValueType) { _ILGenerator.Emit(OpCodes.Box, _parameter.ParameterType); }
                        _ILGenerator.Emit(OpCodes.Stelem_Ref);
                    }
                    _ILGenerator.Emit(OpCodes.Callvirt, Metadata<Action<object, object[]>>.Method(_Action => _Action.Invoke(Metadata<object>.Value, Metadata<object[]>.Value)));
                });
            }
        }

        static public Advisor Returning(this Advisor.IAfter @this, Action<object, object[], object> advice)
        {
            var _type = __Advisor.m_Module.DefineType(Guid.NewGuid().ToString("N"), TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.Public);
            _type.DefineField(Guid.NewGuid().ToString("N"), Metadata<Action<object, object[], object>>.Type, FieldAttributes.Static | FieldAttributes.Public);
            var _field = _type.CreateType().GetFields().Single();
            var _signature = @this.Generator.Method.GetParameters();
            if (@this.Generator.Method.IsStatic)
            {
                return @this.Generator.After().Returning(_ILGenerator =>
                {
                    _ILGenerator.Emit(OpCodes.Ldsfld, _field);
                    _ILGenerator.Emit(OpCodes.Ldnull);
                    _ILGenerator.Emit(OpCodes.Ldc_I4, _signature.Length);
                    _ILGenerator.Emit(OpCodes.Newarr, Metadata<object>.Type);
                    foreach (var _parameter in _signature)
                    {
                        _ILGenerator.Emit(OpCodes.Dup);
                        _ILGenerator.Emit(OpCodes.Ldc_I4, _parameter.Position);
                        _ILGenerator.Emit(OpCodes.Ldarg, _parameter.Position);
                        if (_parameter.ParameterType.IsValueType) { _ILGenerator.Emit(OpCodes.Box, _parameter.ParameterType); }
                        _ILGenerator.Emit(OpCodes.Stelem_Ref);
                    }
                    if (@this.Generator.Method is ConstructorInfo || (@this.Generator.Method as MethodInfo).ReturnType == Metadata.Void) { _ILGenerator.Emit(OpCodes.Ldnull); }
                    else
                    {
                        _ILGenerator.Emit(OpCodes.Ldarg, _signature.Length);
                        if ((@this.Generator.Method as MethodInfo).ReturnType.IsValueType) { _ILGenerator.Emit(OpCodes.Box, (@this.Generator.Method as MethodInfo).ReturnType); }
                    }
                    _ILGenerator.Emit(OpCodes.Callvirt, Metadata<Action<object, object[], object>>.Method(_Action => _Action.Invoke(Metadata<object>.Value, Metadata<object[]>.Value, Metadata<object>.Value)));
                });
            }
            else
            {
                return @this.Generator.After().Returning(_ILGenerator =>
                {
                    _ILGenerator.Emit(OpCodes.Ldsfld, _field);
                    _ILGenerator.Emit(OpCodes.Ldarg_0);
                    _ILGenerator.Emit(OpCodes.Ldc_I4, _signature.Length);
                    _ILGenerator.Emit(OpCodes.Newarr, Metadata<object>.Type);
                    foreach (var _parameter in _signature)
                    {
                        _ILGenerator.Emit(OpCodes.Dup);
                        _ILGenerator.Emit(OpCodes.Ldc_I4, _parameter.Position + 1);
                        _ILGenerator.Emit(OpCodes.Ldarg, _parameter.Position);
                        if (_parameter.ParameterType.IsValueType) { _ILGenerator.Emit(OpCodes.Box, _parameter.ParameterType); }
                        _ILGenerator.Emit(OpCodes.Stelem_Ref);
                    }
                    if (@this.Generator.Method is ConstructorInfo || (@this.Generator.Method as MethodInfo).ReturnType == Metadata.Void) { _ILGenerator.Emit(OpCodes.Ldnull); }
                    else
                    {
                        _ILGenerator.Emit(OpCodes.Ldarg, _signature.Length + 1);
                        if ((@this.Generator.Method as MethodInfo).ReturnType.IsValueType) { _ILGenerator.Emit(OpCodes.Box, (@this.Generator.Method as MethodInfo).ReturnType); }
                    }
                    _ILGenerator.Emit(OpCodes.Callvirt, Metadata<Action<object, object[], object>>.Method(_Action => _Action.Invoke(Metadata<object>.Value, Metadata<object[]>.Value, Metadata<object>.Value)));
                });
            }
        }

        static public Advisor Throwing(this Advisor.IAfter @this, Action<object, object[], Exception> advice)
        {
            var _type = __Advisor.m_Module.DefineType(Guid.NewGuid().ToString("N"), TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.Public);
            _type.DefineField(Guid.NewGuid().ToString("N"), Metadata<Action<object, object[], object>>.Type, FieldAttributes.Static | FieldAttributes.Public);
            var _field = _type.CreateType().GetFields().Single();
            var _signature = @this.Generator.Method.GetParameters();
            if (@this.Generator.Method.IsStatic)
            {
                return @this.Generator.After().Throwing(_ILGenerator =>
                {
                    _ILGenerator.Emit(OpCodes.Ldsfld, _field);
                    _ILGenerator.Emit(OpCodes.Ldnull);
                    _ILGenerator.Emit(OpCodes.Ldc_I4, _signature.Length);
                    _ILGenerator.Emit(OpCodes.Newarr, Metadata<object>.Type);
                    foreach (var _parameter in _signature)
                    {
                        _ILGenerator.Emit(OpCodes.Dup);
                        _ILGenerator.Emit(OpCodes.Ldc_I4, _parameter.Position);
                        _ILGenerator.Emit(OpCodes.Ldarg, _parameter.Position);
                        if (_parameter.ParameterType.IsValueType) { _ILGenerator.Emit(OpCodes.Box, _parameter.ParameterType); }
                        _ILGenerator.Emit(OpCodes.Stelem_Ref);
                    }
                    _ILGenerator.Emit(OpCodes.Ldarg, _signature.Length);
                    _ILGenerator.Emit(OpCodes.Callvirt, Metadata<Action<object, object[], Exception>>.Method(_Action => _Action.Invoke(Metadata<object>.Value, Metadata<object[]>.Value, Metadata<Exception>.Value)));
                });
            }
            else
            {
                return @this.Generator.After().Throwing(_ILGenerator =>
                {
                    _ILGenerator.Emit(OpCodes.Ldsfld, _field);
                    _ILGenerator.Emit(OpCodes.Ldarg_0);
                    _ILGenerator.Emit(OpCodes.Ldc_I4, _signature.Length);
                    _ILGenerator.Emit(OpCodes.Newarr, Metadata<object>.Type);
                    foreach (var _parameter in _signature)
                    {
                        _ILGenerator.Emit(OpCodes.Dup);
                        _ILGenerator.Emit(OpCodes.Ldc_I4, _parameter.Position + 1);
                        _ILGenerator.Emit(OpCodes.Ldarg, _parameter.Position);
                        if (_parameter.ParameterType.IsValueType) { _ILGenerator.Emit(OpCodes.Box, _parameter.ParameterType); }
                        _ILGenerator.Emit(OpCodes.Stelem_Ref);
                    }
                    _ILGenerator.Emit(OpCodes.Ldarg, _signature.Length + 1);
                    _ILGenerator.Emit(OpCodes.Callvirt, Metadata<Action<object, object[], Exception>>.Method(_Action => _Action.Invoke(Metadata<object>.Value, Metadata<object[]>.Value, Metadata<Exception>.Value)));
                });
            }
        }
    }
}

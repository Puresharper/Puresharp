using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    static internal class Validation
    {
        static public readonly ModuleBuilder Module = AppDomain.CurrentDomain.DefineDynamicModule();
    }

    static public class Validation<TAttribute>
        where TAttribute : Attribute
    {
        static public Func<IAdvice> With<TValidator>(MethodBase method)
            where TValidator : class, IValidator, new()
        {
            var _signature = method.GetParameters();
            var _type = Validation.Module.DefineType(Guid.NewGuid().ToString("N"), TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Serializable, Metadata<Advice>.Type, new Type[] { Metadata<IAdvice>.Type });
            var _field = _type.DefineField("m_Index", Metadata<int>.Type, FieldAttributes.Private);
            var _method = _type.DefineMethod("IAdvice.Argument`1", MethodAttributes.Private | MethodAttributes.Virtual, CallingConventions.HasThis);
            var _genericity = _method.DefineGenericParameters("T")[0];
            _method.SetParameters(_genericity.MakeByRefType());
            var _body = _method.GetILGenerator();
            var _table = new Label[_signature.Length];
            for (var _index = 0; _index < _signature.Length; _index++) { _table[_index] = _body.DefineLabel(); }
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Ldfld, _field);
            _body.Emit(OpCodes.Switch, _table);
            _body.Emit(OpCodes.Ret);
            foreach (var _parameter in _signature)
            {
                _body.MarkLabel(_table[_parameter.Position]);
                if (Attribute<TAttribute>.On(method, _parameter))
                {
                    _body.Emit(OpCodes.Ldsfld, Metadata.Field(() => Singleton<TValidator>.Value));
                    _body.Emit(OpCodes.Ldsfld, Runtime.Inventory.Parameter(_parameter));
                    _body.Emit(OpCodes.Ldarg_1);
                    _body.Emit(OpCodes.Ldobj, _genericity);
                    _body.Emit(OpCodes.Callvirt, Metadata<IValidator>.Method(_Validator => _Validator.Validate(Metadata<ParameterInfo>.Value, Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_genericity));
                }
                _body.Emit(OpCodes.Ret);
            }
            _type.DefineMethodOverride(_method, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Argument(ref Metadata<object>.Value)).GetGenericMethodDefinition());
            return Expression.Lambda<Func<IAdvice>>(Expression.New(_type.CreateType())).Compile();
        }
    }
}

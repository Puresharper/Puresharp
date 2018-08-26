using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    static public class __Advisor
    {
        static public readonly ModuleBuilder Module = AppDomain.CurrentDomain.DefineDynamicModule();

        static public Advisor Around(this Advisor.IGenerator @this, Func<IAdvice> advise)
        {
            return new Advisor(advise);
        }

        //static public Advisor Before(this Advisor.IGenerator @this, Action<ILGenerator> advise)
        //{
        //    //generate implementation of IAdvice where begin method call a static method with the original signature!
        //    //generate dynamic method instead of static!? to manage private types?
        //    throw new NotImplementedException();
        //}

        //static public Advisor Before(this Advisor.IGenerator @this, Func<Advisor.Invocation, Expression> advise)
        //{
        //    //delegate to ilgenerator style!?
        //    throw new NotImplementedException();
        //}

        //static public Advisor.After After(this Advisor.IGenerator @this)
        //{
        //    return new Advisor.After(@this);
        //}

        //static public Advisor After(this Advisor.IGenerator @this, Action<ILGenerator> advise)
        //{
        //    throw new NotImplementedException();
        //}

        //static public Advisor After(this Advisor.IGenerator @this, Func<Advisor.Invocation, Expression> advise)
        //{
        //    throw new NotImplementedException();
        //}

        //static public Advisor Throwing(this Advisor.IAfter @this, Action<ILGenerator> advise)
        //{
        //    throw new NotImplementedException();
        //}

        //static public Advisor Throwing(this Advisor.IAfter @this, Func<Advisor.Execution.Throwing, Expression> advise)
        //{
        //    throw new NotImplementedException();
        //}

        //static public Advisor Returning(this Advisor.IAfter @this, Action<ILGenerator> advise)
        //{
        //    throw new NotImplementedException();
        //}

        //static public Advisor Returning(this Advisor.IAfter @this, Func<Advisor.Execution.Returning, Expression> advise)
        //{
        //    throw new NotImplementedException();
        //}

        static public Advisor.Parameter<T> Parameter<T>(this Advisor.IGenerator @this)
            where T : Attribute
        {
            return new Advisor.Parameter<T>(@this);
        }
        
        static public Advisor Validate<T>(this Advisor.IParameter<T> @this, IValidator validator)
            where T : Attribute
        {
            return @this.Validate((_Parameter, _Attribute) => validator);
        }

        static public Advisor Validate<T>(this Advisor.IParameter<T> @this, Func<IValidator> validator)
            where T : Attribute
        {
            return @this.Validate((_Parameter, _Attribute) => validator());
        }

        static public Advisor Validate<T>(this Advisor.IParameter<T> @this, Func<ParameterInfo, IValidator> validator)
            where T : Attribute
        {
            return @this.Validate((_Parameter, _Attribute) => validator(_Parameter));
        }

        static public Advisor Validate<T>(this Advisor.IParameter<T> @this, Func<ParameterInfo, T, IValidator> validator)
            where T : Attribute
        {
            var _signature = @this.Generator.Method.GetParameters();
            if (!_signature.Any(_Parameter => Attribute<T>.On(@this.Generator.Method, _Parameter))) { return @this.Generator.Around(Advisor.Null); }
            var _type = __Advisor.Module.DefineType(Guid.NewGuid().ToString("N"), TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Serializable, Metadata<Advice>.Type, new Type[] { Metadata<IAdvice>.Type });
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
                if (Attribute<T>.On(@this.Generator.Method, _parameter))
                {
                    _body.Emit(OpCodes.Ldsfld, _type.DefineField($"<{ _parameter.Name }>", Metadata<IValidator>.Type, FieldAttributes.Static | FieldAttributes.Private));
                    _body.Emit(OpCodes.Ldarg_1);
                    _body.Emit(OpCodes.Ldobj, _genericity);
                    _body.Emit(OpCodes.Callvirt, Metadata<IValidator>.Method(_Validator => _Validator.Validate(Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_genericity));
                }
                _body.Emit(OpCodes.Ret);
            }
            _type.DefineMethodOverride(_method, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Argument(ref Metadata<object>.Value)).GetGenericMethodDefinition());
            var _validation = _type.CreateType();
            foreach (var _parameter in _signature)
            {
                if (Attribute<T>.On(@this.Generator.Method, _parameter))
                {
                    _validation.GetField($"<{ _parameter.Name }>", BindingFlags.Static | BindingFlags.NonPublic).SetValue(null, validator(_parameter, Attribute<T>.From(@this.Generator.Method, _parameter)));
                }
            }
            return @this.Generator.Around(Expression.Lambda<Func<IAdvice>>(Expression.New(_validation)).Compile());
        }
    }
}

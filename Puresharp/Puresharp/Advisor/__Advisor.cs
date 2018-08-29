using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    static public class __Advisor
    {
        static public Advisor Around(this Advisor.IGenerator @this, Func<IAdvice> advise)
        {
            return new Advisor(advise);
        }

        static private Advisor Create(this Advisor.IGenerator @this, Action<TypeBuilder, FieldBuilder, List<FieldBuilder>> advise)
        {
            var _signature = @this.Method.GetParameters();
            var _type = Aspect.Module.DefineType(Guid.NewGuid().ToString("N"), TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Serializable, Metadata<Advice>.Type, new Type[] { Metadata<IAdvice>.Type });
            var _field = _type.DefineField("<Index>", Metadata<int>.Type, FieldAttributes.Private);
            var _instance = @this.Method.IsStatic ? null : _type.DefineField("<This>", @this.Method.DeclaringType, FieldAttributes.Private);
            var _method = null as MethodBuilder;
            var _body = null as ILGenerator;
            var _genericity = null as GenericTypeParameterBuilder;
            var _table = new Label[_signature.Length];
            if (_signature.Any())
            {
                _method = _type.DefineMethod("IAdvice.Argument`1", MethodAttributes.Private | MethodAttributes.Virtual, CallingConventions.HasThis);
                _genericity = _method.DefineGenericParameters("T")[0];
                _method.SetParameters(_genericity.MakeByRefType());
                _body = _method.GetILGenerator();
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
            }
            var _parameters = new LinkedList<Type>();
            var _arguments = new List<FieldBuilder>();
            foreach (var _parameter in _signature)
            {
                var _argument = _type.DefineField("<This>", _parameter.ParameterType, FieldAttributes.Private);
                _parameters.AddLast(_parameter.ParameterType);
                _arguments.Add(_argument);
                _body.MarkLabel(_table[_parameter.Position]);
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldarg_1);
                _body.Emit(OpCodes.Ldobj, _genericity);
                _body.Emit(OpCodes.Stfld, _argument);
                _body.Emit(OpCodes.Ret);
            }
            _type.DefineMethodOverride(_method, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Argument(ref Metadata<object>.Value)).GetGenericMethodDefinition());
            if (_instance != null)
            {
                _parameters.AddFirst(@this.Method.DeclaringType);
                _method = _type.DefineMethod("IAdvice.Instance`1", MethodAttributes.Private | MethodAttributes.Virtual, CallingConventions.HasThis);
                _genericity = _method.DefineGenericParameters("T")[0];
                _method.SetParameters(_genericity);
                _body = _method.GetILGenerator();
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldarg_1);
                _body.Emit(OpCodes.Stfld, _instance);
                _body.Emit(OpCodes.Ret);
                _type.DefineMethodOverride(_method, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Instance(Metadata<object>.Value)).GetGenericMethodDefinition());
            }
            advise(_type, _instance, _arguments);
            return @this.Around(Expression.Lambda<Func<IAdvice>>(Expression.New(_type.CreateType())).Compile());
        }

        static private Advisor Before(this Advisor.IGenerator @this, Action<MethodBuilder> advise)
        {
            var _signature = @this.Method.GetParameters().Select(_Parameter => _Parameter.ParameterType).ToArray();
            return @this.Create((_Type, _Instance, _Arguments) =>
            {
                var _advice = _Type.DefineMethod("<Advice>", MethodAttributes.Static | MethodAttributes.Private, CallingConventions.Standard, Metadata.Void, @this.Method.IsStatic ? _signature : new Type[] { @this.Method.DeclaringType }.Concat(_signature).ToArray());
                advise(_advice);
                var _method = _Type.DefineMethod("IAdvice.Begin", MethodAttributes.Private | MethodAttributes.Virtual, CallingConventions.HasThis, Metadata.Void, Type.EmptyTypes);
                var _body = _method.GetILGenerator();
                if (_Instance != null)
                {
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldfld, _Instance);
                }
                foreach (var _argument in _Arguments)
                {
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldfld, _argument);
                }
                _body.Emit(OpCodes.Call, _advice);
                _body.Emit(OpCodes.Ret);
                _Type.DefineMethodOverride(_method, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Begin()));
            });
        }

        static public Advisor Before(this Advisor.IGenerator @this, Action<ILGenerator> advise)
        {
            return @this.Before(new Action<MethodBuilder>(_Method =>
            {
                var _body = _Method.GetILGenerator();
                advise(_body);
                _body.Emit(OpCodes.Ret);
            }));
        }

        static public Advisor Before(this Advisor.IGenerator @this, Func<Advisor.Invocation, Expression> advise)
        {
            var _signature = @this.Method.GetParameters().Select(_Parameter => Expression.Parameter(_Parameter.ParameterType));
            return @this.Before(new Action<MethodBuilder>(_Method =>
            {
                if (@this.Method.IsStatic) { Expression.Lambda(advise(new Advisor.Invocation(@this.Method, null, new Collection<Expression>(_signature))), _signature).CompileToMethod(_Method); }
                else
                {
                    var _instance = Expression.Parameter(@this.Method.DeclaringType);
                    Expression.Lambda(advise(new Advisor.Invocation(@this.Method, _instance, new Collection<Expression>(_signature))), new ParameterExpression[] { _instance }.Concat(_signature)).CompileToMethod(_Method);
                }
            }));
        }

        static public Advisor.After After(this Advisor.IGenerator @this)
        {
            return new Advisor.After(@this);
        }

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
        //}

        //static public Advisor Returning(this Advisor.IAfter @this, Func<Advisor.Execution.Returning, Expression> advise)
        //{  
        //}

        static public Advisor.Parameter Parameter(this Advisor.IGenerator @this)
        {
            return new Advisor.Parameter(@this);
        }

        static public Advisor.Parameter<T> Parameter<T>(this Advisor.IGenerator @this)
            where T : Attribute
        {
            return new Advisor.Parameter<T>(@this);
        }
    }
}

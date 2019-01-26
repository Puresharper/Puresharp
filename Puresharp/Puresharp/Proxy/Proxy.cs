using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Puresharp
{
    abstract public partial class Proxy : IDisposable
    {
        static private ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule();

        static internal Expression Authentic<T>(MethodInfo method, Expression instance, IEnumerable<Expression> arguments, Expression body)
            where T : class
        {
            var _signature = method.GetParameters();
            var _type = Proxy.m_Module.DefineType(Guid.NewGuid().ToString("N"), TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable, Metadata<object>.Type, new Type[] { Metadata<IActivity>.Type });
            var _this = _type.DefineField("<This>", Metadata<T>.Type, FieldAttributes.Private);
            var _method = _type.DefineMethod("IAdvice.Instance`1", MethodAttributes.Private | MethodAttributes.Virtual, CallingConventions.HasThis, Metadata.Void, new Type[] { Metadata<T>.Type });
            var _genericity = _method.DefineGenericParameters("T")[0];
            _method.SetParameters(_genericity);
            var _body = _method.GetILGenerator();
            _body.Emit(OpCodes.Ldarg_0);
            _body.Emit(OpCodes.Ldarg_1);
            _body.Emit(OpCodes.Stfld, _this);
            _body.Emit(OpCodes.Ret);
            _type.DefineMethodOverride(_method, Metadata<IActivity>.Method(_IActivity => _IActivity.Instance(Metadata<object>.Value)).GetGenericMethodDefinition());
            var _move = _type.DefineField("<Move>", Metadata<int>.Type, FieldAttributes.Private);
            var _structure = new List<FieldBuilder>();
            for (var _index = 0; _index < _signature.Length; _index++) { _structure.Add(_type.DefineField($"m_{ _signature[_index].Name }", _signature[_index].ParameterType, FieldAttributes.Private)); }
            _method = _type.DefineMethod("IActivity.Argument`1", MethodAttributes.Private | MethodAttributes.Virtual, CallingConventions.HasThis);
            _genericity = _method.DefineGenericParameters("T")[0];
            _method.SetParameters(new Type[] { _genericity });
            _body = _method.GetILGenerator();
            if (_signature.Any())
            {
                var _table = new Label[_signature.Length];
                for (var _index = 0; _index < _signature.Length; _index++) { _table[_index] = _body.DefineLabel(); }
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldfld, _move);
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldfld, _move);
                _body.Emit(OpCodes.Ldc_I4_1);
                _body.Emit(OpCodes.Add);
                _body.Emit(OpCodes.Stfld, _move);
                _body.Emit(OpCodes.Switch, _table);
                _body.Emit(OpCodes.Ret);
                for (var _index = 0; _index < _signature.Length; _index++)
                {
                    _body.MarkLabel(_table[_index]);
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldarg_1);
                    _body.Emit(OpCodes.Stfld, _structure[_index]);
                    _body.Emit(OpCodes.Ret);
                }
            }
            else { _body.Emit(OpCodes.Ret); }
            _type.DefineMethodOverride(_method, Metadata<IActivity>.Method(_IActivity => _IActivity.Argument(Metadata<object>.Value)).GetGenericMethodDefinition());
            var _action = _type.DefineMethod(nameof(IActivity.Invoke), MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.HideBySig, CallingConventions.HasThis);
            var _function = _type.DefineMethod($"{ nameof(IActivity.Invoke) }`1", MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.HideBySig, CallingConventions.HasThis);
            _function.SetReturnType(_function.DefineGenericParameters("T")[0]);
            if (method.ReturnType == Metadata.Void)
            {
                _body = _action.GetILGenerator();
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldfld, _this);
                foreach (var _field in _structure)
                {
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldfld, _field);
                }
                switch (IntPtr.Size)
                {
                    case 4: _body.Emit(OpCodes.Ldc_I4, (body as MethodCallExpression).Method.GetFunctionPointer().ToInt32()); break;
                    case 8: _body.Emit(OpCodes.Ldc_I8, (body as MethodCallExpression).Method.GetFunctionPointer().ToInt64()); break;
                    default: throw new NotSupportedException();
                }
                _body.EmitCalli(OpCodes.Calli, CallingConventions.Standard, method.ReturnType, (body as MethodCallExpression).Method.GetParameters().Select(_Parameter => _Parameter.ParameterType).ToArray(), null);
                _body.Emit(OpCodes.Ret);
                _body = _function.GetILGenerator();
                _body.Emit(OpCodes.Newobj, Metadata.Constructor(() => new InvalidOperationException()));
                _body.Emit(OpCodes.Throw);
            }
            else
            {
                _body = _action.GetILGenerator();
                _body.Emit(OpCodes.Newobj, Metadata.Constructor(() => new InvalidOperationException()));
                _body.Emit(OpCodes.Throw);
                _body = _function.GetILGenerator();
                _body.Emit(OpCodes.Ldarg_0);
                _body.Emit(OpCodes.Ldfld, _this);
                foreach (var _field in _structure)
                {
                    _body.Emit(OpCodes.Ldarg_0);
                    _body.Emit(OpCodes.Ldfld, _field);
                }
                switch (IntPtr.Size)
                {
                    case 4: _body.Emit(OpCodes.Ldc_I4, (body as MethodCallExpression).Method.GetFunctionPointer().ToInt32()); break;
                    case 8: _body.Emit(OpCodes.Ldc_I8, (body as MethodCallExpression).Method.GetFunctionPointer().ToInt64()); break;
                    default: throw new NotSupportedException();
                }
                _body.EmitCalli(OpCodes.Calli, CallingConventions.Standard, method.ReturnType, (body as MethodCallExpression).Method.GetParameters().Select(_Parameter => _Parameter.ParameterType).ToArray(), null);
                _body.Emit(OpCodes.Ret);
            }
            _type.DefineMethodOverride(_action, Metadata<IActivity>.Method(_IActivity => _IActivity.Invoke()));
            _type.DefineMethodOverride(_function, Metadata<IActivity>.Method(_IActivity => _IActivity.Invoke<object>()).GetGenericMethodDefinition());
            _method = _type.DefineMethod("IDisposable.Dispose", MethodAttributes.Private | MethodAttributes.Virtual, CallingConventions.HasThis);
            _body = _method.GetILGenerator();
            _body.Emit(OpCodes.Ret);
            _type.DefineMethodOverride(_method, Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()));
            return Expression.New(_type.CreateType());
        }

        internal class Manager : Aspect
        {
            private Func<MethodBase, IntPtr, IntPtr> m_Override;

            public Manager(Func<MethodBase, IntPtr, IntPtr> @override)
            {
                this.m_Override = @override;
            }

            override public IEnumerable<Advisor> Manage(MethodBase method)
            {
                throw new InvalidOperationException();
            }

            public IntPtr Override(MethodBase method, IntPtr pointer)
            {
                return this.m_Override(method, pointer);
            }
        }

        private sealed class Patcher : ExpressionVisitor
        {
            static private MethodInfo Method(MethodBase method, IntPtr pointer)
            {
                var _type = method.ReturnType();
                var _signature = method.Signature();
                var _method = new DynamicMethod(string.Empty, _type, _signature, method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.Emit(_signature, false);
                _body.Emit(pointer, _type, _signature);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            }

            static public Expression Patch(Expression expression, MethodBase method, IntPtr pointer)
            {
                return new Proxy.Patcher(method, pointer).Visit(expression);
            }

            private readonly MethodBase m_Method;
            private readonly IntPtr m_Pointer;

            private Patcher(MethodBase method, IntPtr pointer)
            {
                this.m_Method = method;
                this.m_Pointer = pointer;
            }

            override protected Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method == this.m_Method)
                {
                    if (this.m_Method.IsStatic) { return Expression.Call(Proxy.Patcher.Method(this.m_Method, this.m_Pointer), node.Arguments); }
                    return Expression.Call(Proxy.Patcher.Method(this.m_Method, this.m_Pointer), new Expression[] { node.Object }.Concat(node.Arguments));
                }
                return base.VisitMethodCall(node);
            }
        }

        private Proxy.Manager m_Manager;

        protected Proxy()
        {
            this.m_Manager = new Proxy.Manager(this.Override);
        }

        private IntPtr Override(MethodBase method, IntPtr pointer)
        {
            var _type = method.ReturnType();
            var _signature = method.Signature();
            var _parameters = new Collection<ParameterExpression>(_signature.Select(_Type => Expression.Parameter(_Type)).ToArray());
            var _method = new DynamicMethod(string.Empty, _type, _signature, method.DeclaringType, true);
            var _body = _method.GetILGenerator();
            _body.Emit(_signature, false);
            _body.Emit(pointer, _type, _signature);
            _body.Emit(OpCodes.Ret);
            var _invocation = _signature.Instance == null ? this.Override(method, null, _parameters, Expression.Call(_method, _parameters)) : this.Override(method, _parameters[0], _parameters.Skip(1), Expression.Call(_method, _parameters));
            if (_invocation == null) { return pointer; }
            _method = new DynamicMethod(string.Empty, _type, _signature, method.DeclaringType, true);
            _body = _method.GetILGenerator();
            _body.Emit(_signature, false);
            _body.Emit(OpCodes.Call, Expression.Lambda(_invocation, _parameters).CompileToMethod());
            _body.Emit(OpCodes.Ret);
            _method.Prepare();
            return _method.GetFunctionPointer();
        }

        abstract protected Expression Override(MethodBase method, Expression instance, IEnumerable<Expression> arguments, Expression body);

        public void Weave<T>()
            where T : Pointcut, new()
        {
            this.m_Manager.Weave<T>();
        }

        public void Release<T>()
            where T : Pointcut, new()
        {
            this.m_Manager.Release<T>();
        }

        public void Release()
        {
            this.m_Manager.Release();
        }

        public void Dispose()
        {
            this.m_Manager.Dispose();
        }
    }

    public sealed class Proxy<T> : Proxy
        where T : class
    {
        private class Methods : Pointcut
        {
            override public bool Match(MethodBase method)
            {
                return method is MethodInfo && !method.IsStatic && Metadata<T>.Type.IsAssignableFrom(method.DeclaringType);
            }
        }

        static Proxy()
        {
            Proxy<T>.m_Singleton.Weave<Proxy<T>.Methods>();
        }

        private class Predicate
        {
            private WeakReference<T> m_Reference;

            public Predicate(T instance)
            {
                this.m_Reference = new WeakReference<T>(instance);
            }

            public bool Match(T instance)
            {
                if (this.m_Reference.TryGetTarget(out var _instance)) { return object.ReferenceEquals(instance, _instance); }
                while (true)
                {
                    var _source = Proxy<T>.m_Overriding;
                    var _destination = new LinkedList<Override<T>>();
                    foreach (var _item in _source)
                    {
                        if (object.ReferenceEquals(_item.Predicate.Target, this)) { continue; }
                        _destination.AddLast(_item);
                    }
                    if (object.ReferenceEquals(Interlocked.CompareExchange(ref Proxy<T>.m_Overriding, _destination, _source), _source)) { break; }
                }
                return false;
            }
        }

        static private Proxy<T> m_Singleton = new Proxy<T>();
        static private LinkedList<Override<T>> m_Overriding = new LinkedList<Override<T>>();

        static public void Override(T instance, Func<MethodInfo, Func<IActivity, IActivity>> overrider)
        {
            Proxy<T>.Override(new Proxy<T>.Predicate(instance).Match, overrider);
        }

        static private Override<T> Override(Func<T, bool> predicate, Func<MethodInfo, Func<IActivity, IActivity>> overrider)
        {
            var _override = new Override<T>(predicate, overrider);
            while (true)
            {
                var _source = Proxy<T>.m_Overriding;
                var _destination = new LinkedList<Override<T>>();
                foreach (var _item in _source) { _destination.AddLast(_item); }
                _destination.AddLast(_override);
                if (object.ReferenceEquals(Interlocked.CompareExchange(ref Proxy<T>.m_Overriding, _destination, _source), _source)) { break; }
            }
            return _override;
        }

        static private IActivity Override(MethodInfo method, IActivity activity, T instance)
        {
            var _activity = activity;
            var _overriding = Proxy<T>.m_Overriding;
            foreach (var _override in _overriding) { if (_override.Predicate(instance)) { _activity = _override.Overrider(method)(_activity); } }
            return _activity;
        }

        private Proxy()
        {
        }

        protected override Expression Override(MethodBase method, Expression instance, IEnumerable<Expression> arguments, Expression body)
        {
            var _method = method as MethodInfo;
            if (_method == null) { return body; }
            var _authentic = Expression.Parameter(typeof(IActivity));
            var _proxy = Expression.Parameter(typeof(IActivity));
            var _body = new List<Expression>();
            _body.Add(Expression.Call(_proxy, Metadata<IActivity>.Method(_Activity => _Activity.Instance<T>(Metadata<T>.Value)), instance));
            foreach (var _argument in arguments) { _body.Add(Expression.Call(_proxy, Metadata<IActivity>.Method(_Activity => _Activity.Argument<object>(Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_argument.Type), _argument)); }
            if (_method.ReturnType == Metadata.Void)
            {
                _body.Add(Expression.Call(_proxy, Metadata<IActivity>.Method(_Activity => _Activity.Invoke())));
                return Expression.Block
                (
                    new ParameterExpression[] { _authentic, _proxy },
                    Expression.Assign(_authentic, Proxy.Authentic<T>(_method, instance, arguments, body)),
                    Expression.TryFinally
                    (
                        Expression.Block
                        (
                            Expression.Assign
                            (
                                _proxy,
                                Expression.Call
                                (
                                    Metadata.Method(() => Proxy<T>.Override(Metadata<MethodInfo>.Value, Metadata<IActivity>.Value, Metadata<T>.Value)),
                                    Expression.Constant(_method),
                                    _authentic,
                                    instance
                                )
                            ),
                            Expression.IfThenElse
                            (
                                Expression.Call
                                (
                                    Metadata.Method(() => object.ReferenceEquals(Metadata<object>.Value, Metadata<object>.Value)),
                                    Expression.TypeAs(_authentic, Metadata<object>.Type),
                                    Expression.TypeAs(_proxy, Metadata<object>.Type)
                                ),
                                body,
                                Expression.Block(_body)
                            )
                        ),
                        Expression.IfThen
                        (
                            Expression.NotEqual(_proxy, Expression.Constant(null)),
                            Expression.Call(Expression.TypeAs(_proxy, Metadata<IActivity>.Type), Metadata<IDisposable>.Method(_Disposable => _Disposable.Dispose()))
                        )
                    )
                );
            }
            else
            {
                var _return = Expression.Parameter(_method.ReturnType);
                _body.Add(Expression.Assign(_return, Expression.Call(_proxy, Metadata<IActivity>.Method(_Activity => _Activity.Invoke<object>()).GetGenericMethodDefinition().MakeGenericMethod(_return.Type))));
                return Expression.Block
                (
                    new ParameterExpression[] { _return, _authentic, _proxy },
                    Expression.Assign(_authentic, Proxy.Authentic<T>(_method, instance, arguments, body)),
                    Expression.TryFinally
                    (
                        Expression.Block
                        (
                            Expression.Assign
                            (
                                _proxy,
                                Expression.Call
                                (
                                    Metadata.Method(() => Proxy<T>.Override(Metadata<MethodInfo>.Value, Metadata<IActivity>.Value, Metadata<T>.Value)),
                                    Expression.Constant(_method),
                                    _authentic,
                                    instance
                                )
                            ),
                            Expression.IfThenElse
                            (
                                Expression.Call
                                (
                                    Metadata.Method(() => object.ReferenceEquals(Metadata<object>.Value, Metadata<object>.Value)),
                                    Expression.TypeAs(_authentic, Metadata<object>.Type),
                                    Expression.TypeAs(_proxy, Metadata<object>.Type)
                                ),
                                Expression.Assign(_return, body),
                                Expression.Block(_body)
                            )
                        ),
                        Expression.IfThen
                        (
                            Expression.NotEqual(_proxy, Expression.Constant(null)),
                            Expression.Call(Expression.TypeAs(_proxy, Metadata<IActivity>.Type), Metadata<IDisposable>.Method(_Disposable => _Disposable.Dispose()))
                        )
                    ),
                    _return
                );
            }
        }
    }
}

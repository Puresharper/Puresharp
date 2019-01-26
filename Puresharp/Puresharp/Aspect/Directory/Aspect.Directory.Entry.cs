using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace Puresharp
{
    abstract public partial class Aspect
    {
        static private partial class Directory
        {
            private sealed partial class Entry : IEnumerable<Aspect>
            {
                static private readonly ModuleBuilder Module = AppDomain.CurrentDomain.DefineDynamicModule();

                static private string Identity(Type type)
                {
                    var _index = type.Name.IndexOf('`');
                    var _name = _index < 0 ? type.Name : type.Name.Substring(0, _index);
                    if (type.GetGenericArguments().Length == 0) { return string.Concat("<", _name, ">"); }
                    _name = string.Concat(_name, "<", type.GetGenericArguments().Length.ToString(), ">");
                    return string.Concat("<", _name, string.Concat("<", string.Concat(type.GetGenericArguments().Select(_argument => string.Concat("<", _argument.Name, ">"))), ">"), ">");
                }

                static private string Identity(MethodBase method)
                {
                    return string.Concat("<", method.IsConstructor ? method.DeclaringType.Name : method.Name, method.GetGenericArguments().Length > 0 ? string.Concat("<", method.GetGenericArguments().Length, ">") : string.Empty, method.GetParameters().Length > 0 ? string.Concat("<", string.Concat(method.GetParameters().Select(_parameter => Identity(_parameter.ParameterType))), ">") : string.Empty, ">");
                }

                static private MethodInfo Update(MethodBase method)
                {
                    foreach (var _instruction in method.Body())
                    {
                        if (_instruction.Code == OpCodes.Ldsfld)
                        {
                            var _field = _instruction.Value as FieldInfo;
                            if (_field.Name == "<Pointer>") { return _field.DeclaringType.GetMethod("<Update>", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly); }
                        }
                    }
                    return null;
                }

                static private FieldInfo Authentic(MethodBase method)
                {
                    foreach (var _instruction in method.Body())
                    {
                        if (_instruction.Code == OpCodes.Ldsfld)
                        {
                            var _field = _instruction.Value as FieldInfo;
                            if (_field.Name == "<Pointer>") { return _field.DeclaringType.GetField("<Authentic>", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly); }
                        }
                    }
                    return null;
                }

                public readonly Type Type;
                public readonly MethodBase Method;
                public readonly Aspect.Activity Activity;
                private readonly LinkedList<Aspect> m_Aspectization;
                private readonly LinkedList<MethodInfo> m_Sequence;
                private readonly Dictionary<Aspect, Aspect.Activity> m_Dictionary;
                private readonly IntPtr m_Pointer;
                private readonly Action<IntPtr> m_Update;
                private readonly FieldInfo m_Factory;

                internal Entry(Type type, MethodBase method, Aspect.Activity activity)
                {
                    this.Type = type;
                    this.Method = method;
                    this.Activity = activity;
                    this.m_Aspectization = new LinkedList<Aspect>();
                    this.m_Dictionary = new Dictionary<Aspect, Aspect.Activity>();
                    var _update = Aspect.Directory.Entry.Update(method);
                    if (_update == null) { throw new NotSupportedException(string.Format($"Method '{ method.Name }' declared in type '{ method.DeclaringType.AssemblyQualifiedName }' is not managed by Puresharp and cannot be supervised. Please install Puresharp nuget package on '{ method.DeclaringType.Assembly.FullName }' to make it supervisable.")); }
                    this.m_Update = Delegate.CreateDelegate(Metadata<Action<IntPtr>>.Type, _update) as Action<IntPtr>;
                    this.m_Pointer = (IntPtr)Aspect.Directory.Entry.Authentic(method).GetValue(null);
                    this.m_Sequence = new LinkedList<MethodInfo>();
                    var _attribute = method.Attribute<AsyncStateMachineAttribute>();
                    if (_attribute == null) { return; }
                    this.m_Factory = (_attribute.StateMachineType.IsGenericTypeDefinition ? _attribute.StateMachineType.MakeGenericType((method.IsGenericMethod ? type.GetGenericArguments().Concat(method.GetGenericArguments()) : type.GetGenericArguments()).ToArray()) : _attribute.StateMachineType).GetField("<Factory>", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
                }

                private MethodInfo Decorate(IntPtr pointer, Func<IAdvice> advise)
                {
                    var _type = this.Method.ReturnType();
                    var _parameters = this.Method.GetParameters();
                    var _signature = this.Method.Signature();
                    var _method = new DynamicMethod(string.Empty, _type, _signature, this.Method.DeclaringType, true);
                    var _body = _method.GetILGenerator();
                    var _factory = _body.DeclareLocal(Metadata<Func<IAdvice>>.Type);
                    var _backup = _body.DeclareLocal(Metadata<Exception>.Type);
                    var _exception = _body.DeclareLocal(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Ldsfld, Aspect.Directory.Entry.Module.DefineField(Guid.NewGuid().ToString("N"), advise));
                    _body.Emit(OpCodes.Callvirt, Metadata<Func<IAdvice>>.Method(_Function => _Function.Invoke()));
                    _body.Emit(OpCodes.Stloc_0);
                    if (this.Method.IsStatic)
                    {
                        for (var _index = 0; _index < _signature.Length; _index++)
                        {
                            _body.Emit(OpCodes.Ldloc_0);
                            _body.Emit(OpCodes.Ldarga_S, _index);
                            _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Argument(ref Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_parameters[_index].ParameterType));
                        }
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Begin()));
                        _body.BeginExceptionBlock();
                        _body.Emit(_signature, false);
                        _body.Emit(pointer, _type, _signature);
                        if (_type == Runtime.Void)
                        {
                            var _return = _body.DefineLabel();
                            var _leave = _body.DefineLabel();
                            var _rethrow = _body.DefineLabel();
                            _body.Emit(OpCodes.Ldloc_0);
                            _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Return()));
                            _body.Emit(OpCodes.Leave, _return);
                            _body.BeginCatchBlock(Metadata<Exception>.Type);
                            _body.Emit(OpCodes.Stloc_1);
                            _body.Emit(OpCodes.Ldloc_1);
                            _body.Emit(OpCodes.Stloc_2);
                            _body.Emit(OpCodes.Ldloc_0);
                            _body.Emit(OpCodes.Ldloca_S, 2);
                            _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Throw(ref Metadata<Exception>.Value)));
                            _body.Emit(OpCodes.Ldloc_2);
                            _body.Emit(OpCodes.Brfalse, _leave);
                            _body.Emit(OpCodes.Ldloc_1);
                            _body.Emit(OpCodes.Ldloc_2);
                            _body.Emit(OpCodes.Beq, _rethrow);
                            _body.Emit(OpCodes.Ldloc_2);
                            _body.Emit(OpCodes.Throw);
                            _body.Emit(OpCodes.Br, _leave);
                            _body.MarkLabel(_rethrow);
                            _body.Emit(OpCodes.Rethrow);
                            _body.MarkLabel(_leave);
                            _body.Emit(OpCodes.Leave, _return);
                            _body.BeginFinallyBlock();
                            _body.Emit(OpCodes.Ldloc_0);
                            _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Dispose()));
                            _body.EndExceptionBlock();
                            _body.MarkLabel(_return);
                            _body.Emit(OpCodes.Ret);
                        }
                        else
                        {
                            var _return = _body.DefineLabel();
                            var _leave = _body.DefineLabel();
                            var _rethrow = _body.DefineLabel();
                            _body.DeclareLocal(_type);
                            _body.Emit(OpCodes.Stloc_3);
                            _body.Emit(OpCodes.Ldloc_0);
                            _body.Emit(OpCodes.Ldloca_S, 3);
                            _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Return(ref Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_type));
                            _body.Emit(OpCodes.Leave, _return);
                            _body.BeginCatchBlock(Metadata<Exception>.Type);
                            _body.Emit(OpCodes.Stloc_1);
                            _body.Emit(OpCodes.Ldloc_1);
                            _body.Emit(OpCodes.Stloc_2);
                            _body.Emit(OpCodes.Ldloc_0);
                            _body.Emit(OpCodes.Ldloca_S, 2);
                            _body.Emit(OpCodes.Ldloca_S, 3);
                            _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Throw(ref Metadata<Exception>.Value, ref Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_type));
                            _body.Emit(OpCodes.Ldloc_2);
                            _body.Emit(OpCodes.Brfalse, _leave);
                            _body.Emit(OpCodes.Ldloc_1);
                            _body.Emit(OpCodes.Ldloc_2);
                            _body.Emit(OpCodes.Beq, _rethrow);
                            _body.Emit(OpCodes.Ldloc_2);
                            _body.Emit(OpCodes.Throw);
                            _body.Emit(OpCodes.Br, _leave);
                            _body.MarkLabel(_rethrow);
                            _body.Emit(OpCodes.Rethrow);
                            _body.MarkLabel(_leave);
                            _body.Emit(OpCodes.Leave, _return);
                            _body.BeginFinallyBlock();
                            _body.Emit(OpCodes.Ldloc_0);
                            _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Dispose()));
                            _body.EndExceptionBlock();
                            _body.MarkLabel(_return);
                            _body.Emit(OpCodes.Ldloc_3);
                            _body.Emit(OpCodes.Ret);
                        }
                    }
                    else
                    {
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Ldarg_0);
                        _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Instance(Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(this.Method.DeclaringType));
                        for (var _index = 0; _index < _parameters.Length; _index++)
                        {
                            _body.Emit(OpCodes.Ldloc_0);
                            _body.Emit(OpCodes.Ldarga_S, _index + 1);
                            _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Argument(ref Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_parameters[_index].ParameterType));
                        }
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Begin()));
                        _body.BeginExceptionBlock();
                        _body.Emit(_signature, false);
                        _body.Emit(pointer, _type, _signature);
                        if (_type == Runtime.Void)
                        {
                            var _return = _body.DefineLabel();
                            var _leave = _body.DefineLabel();
                            var _rethrow = _body.DefineLabel();
                            _body.Emit(OpCodes.Ldloc_0);
                            _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Return()));
                            _body.Emit(OpCodes.Leave, _return);
                            _body.BeginCatchBlock(Metadata<Exception>.Type);
                            _body.Emit(OpCodes.Stloc_1);
                            _body.Emit(OpCodes.Ldloc_1);
                            _body.Emit(OpCodes.Stloc_2);
                            _body.Emit(OpCodes.Ldloc_0);
                            _body.Emit(OpCodes.Ldloca_S, 2);
                            _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Throw(ref Metadata<Exception>.Value)));
                            _body.Emit(OpCodes.Ldloc_2);
                            _body.Emit(OpCodes.Brfalse, _leave);
                            _body.Emit(OpCodes.Ldloc_1);
                            _body.Emit(OpCodes.Ldloc_2);
                            _body.Emit(OpCodes.Beq, _rethrow);
                            _body.Emit(OpCodes.Ldloc_2);
                            _body.Emit(OpCodes.Throw);
                            _body.Emit(OpCodes.Br, _leave);
                            _body.MarkLabel(_rethrow);
                            _body.Emit(OpCodes.Rethrow);
                            _body.MarkLabel(_leave);
                            _body.Emit(OpCodes.Leave, _return);
                            _body.BeginFinallyBlock();
                            _body.Emit(OpCodes.Ldloc_0);
                            _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Dispose()));
                            _body.EndExceptionBlock();
                            _body.MarkLabel(_return);
                            _body.Emit(OpCodes.Ret);
                        }
                        else
                        {
                            var _return = _body.DefineLabel();
                            var _leave = _body.DefineLabel();
                            var _rethrow = _body.DefineLabel();
                            _body.DeclareLocal(_type);
                            _body.Emit(OpCodes.Stloc_3);
                            _body.Emit(OpCodes.Ldloc_0);
                            _body.Emit(OpCodes.Ldloca_S, 3);
                            _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Return(ref Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_type));
                            _body.Emit(OpCodes.Leave, _return);
                            _body.BeginCatchBlock(Metadata<Exception>.Type);
                            _body.Emit(OpCodes.Stloc_1);
                            _body.Emit(OpCodes.Ldloc_1);
                            _body.Emit(OpCodes.Stloc_2);
                            _body.Emit(OpCodes.Ldloc_0);
                            _body.Emit(OpCodes.Ldloca_S, 2);
                            _body.Emit(OpCodes.Ldloca_S, 3);
                            _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Throw(ref Metadata<Exception>.Value, ref Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_type));
                            _body.Emit(OpCodes.Ldloc_2);
                            _body.Emit(OpCodes.Brfalse, _leave);
                            _body.Emit(OpCodes.Ldloc_1);
                            _body.Emit(OpCodes.Ldloc_2);
                            _body.Emit(OpCodes.Beq, _rethrow);
                            _body.Emit(OpCodes.Ldloc_2);
                            _body.Emit(OpCodes.Throw);
                            _body.Emit(OpCodes.Br, _leave);
                            _body.MarkLabel(_rethrow);
                            _body.Emit(OpCodes.Rethrow);
                            _body.MarkLabel(_leave);
                            _body.Emit(OpCodes.Leave, _return);
                            _body.BeginFinallyBlock();
                            _body.Emit(OpCodes.Ldloc_0);
                            _body.Emit(OpCodes.Callvirt, Metadata<IAdvice>.Method(_IAdvice => _IAdvice.Dispose()));
                            _body.EndExceptionBlock();
                            _body.MarkLabel(_return);
                            _body.Emit(OpCodes.Ldloc_3);
                            _body.Emit(OpCodes.Ret);
                        }
                    }
                    _method.Prepare();
                    return _method;
                }

                private void Update()
                {
                    var _interception = this.m_Aspectization.Where(_Aspect => _Aspect is Proxy.Manager).Cast<Proxy.Manager>().ToArray();
                    var _aspectization = this.m_Aspectization.Except(_interception).ToArray();
                    var _addin = _aspectization.Where(_Aspect => _Aspect.GetType().GetCustomAttributes(Metadata<Aspect.Addin>.Type, true).Any()).ToArray();
                    var _addon = _aspectization.Where(_Aspect => _Aspect.GetType().GetCustomAttributes(Metadata<Aspect.Addon>.Type, true).Any()).ToArray();
                    var _between = _aspectization.Except(_addin).Except(_addon).ToArray();
                    var _override = _addin.Concat(_between).Concat(_addon).Manage(this.Method).ToArray();
                    this.m_Sequence.Clear();
                    if (this.m_Factory == null)
                    {
                        var _pointer = this.m_Pointer;
                        foreach (var _advisor in _override) { _pointer = this.Decorate(_pointer, _advisor).GetFunctionPointer(); }
                        foreach (var _proxy in _interception) { _pointer = _proxy.Override(this.Method, _pointer); }
                        this.m_Update(_pointer);
                    }
                    else
                    {
                        if (_override.Length == 0) { this.m_Factory.SetValue(null, Advisor.Null); }
                        else { this.m_Factory.SetValue(null, new Func<IAdvice>(new Advisor.Sequence.Factory(_override).Create)); }
                        var _pointer = this.m_Pointer;
                        foreach (var _proxy in _interception) { _pointer = _proxy.Override(this.Method, _pointer); }
                        this.m_Update(_pointer);
                    }
                }

                public void Add(Aspect aspect)
                {
                    if (this.m_Dictionary.ContainsKey(aspect))
                    {
                        this.Update();
                        return;
                    }
                    this.m_Aspectization.AddFirst(aspect);
                    this.m_Dictionary.Add(aspect, null);
                    this.Update();
                }

                public void Remove(Aspect aspect)
                {
                    if (this.m_Dictionary.Remove(aspect))
                    {
                        this.m_Aspectization.Remove(aspect);
                        this.Update();
                    }
                }

                IEnumerator<Aspect> IEnumerable<Aspect>.GetEnumerator()
                {
                    return this.m_Aspectization.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return this.m_Aspectization.GetEnumerator();
                }
            }

            [DebuggerDisplay("{Debugger.Display(this) , nq}")]
            [DebuggerTypeProxy(typeof(Entry.Debugger))]
            private sealed partial class Entry
            {
                private class Debugger
                {
                    static public string Display(Aspect.Directory.Entry map)
                    {
                        return string.Concat(map.Type.Declaration(), ".", map.Method.Declaration(), " = ", map.m_Aspectization.Count.ToString());
                    }

                    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                    private Aspect.Directory.Entry m_Map;

                    public Debugger(Aspect.Directory.Entry map)
                    {
                        this.m_Map = map;
                    }

                    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
                    public Aspect[] View
                    {
                        get { return this.m_Map.m_Aspectization.ToArray(); }
                    }
                }
            }
        }
    }
}
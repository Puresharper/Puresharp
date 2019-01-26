using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Puresharp;

using Assembly = System.Reflection.Assembly;
using MethodBase = System.Reflection.MethodBase;
using MethodInfo = System.Reflection.MethodInfo;
using ParameterInfo = System.Reflection.ParameterInfo;
using BindingFlags = System.Reflection.BindingFlags;
using CallSite = Mono.Cecil.CallSite;

namespace IPuresharp
{
    static internal class Machine
    {
        static public void Manage(this MethodDefinition method)
        {
            var _attribute = method.CustomAttributes.SingleOrDefault(_Attribute => _Attribute.AttributeType.Resolve() == method.Module.Import(typeof(AsyncStateMachineAttribute)).Resolve());
            if (_attribute == null) { return; }
            var _genericity = method.DeclaringType.GenericParameters.Concat(method.GenericParameters).ToArray();
            var _type = _attribute.ConstructorArguments[0].Value as TypeDefinition;
            var _factory = _type.Field<Func<IAdvice>>("<Factory>", FieldAttributes.Public | FieldAttributes.Static);
            var _advice = _type.Field<IAdvice>("<Advice>", FieldAttributes.Public);
            _type.IsBeforeFieldInit = true;
            var _intializer = _type.Initializer();
            _intializer.Body.Emit(OpCodes.Call, Metadata.Property(() => Advisor.Null).GetGetMethod(true));
            _intializer.Body.Emit(OpCodes.Stsfld, _factory.Relative());
            _intializer.Body.Emit(OpCodes.Ret);
            var _constructor = _type.Methods.SingleOrDefault(m => m.IsConstructor && !m.IsStatic);
            if (_constructor == null) { _constructor = _type.Initializer(); }
            _constructor.Body = new MethodBody(_constructor);
            _constructor.Body.Emit(OpCodes.Ldarg_0);
            _constructor.Body.Emit(OpCodes.Call, Metadata.Constructor(() => new object()));
            _constructor.Body.Emit(OpCodes.Ldarg_0);
            _constructor.Body.Emit(OpCodes.Ldsfld, _constructor.Module.Import(_factory.Relative()));
            _constructor.Body.Emit(OpCodes.Callvirt, Metadata<Func<IAdvice>>.Method(_Function => _Function.Invoke()));
            _constructor.Body.Emit(OpCodes.Stfld, _advice.Relative());
            _constructor.Body.Emit(OpCodes.Ret);
            var _move = _type.Methods.Single(_Method => _Method.Name == "MoveNext");
            var _importation = new Importation(method, _move);
            var _task = _move.Body.Variable<Task>();
            var _instance = null as FieldReference;
            if (!method.IsStatic)
            {
                _instance = _type.Fields.SingleOrDefault(_Field => _Field.Name == "<>4__this");
                if (_instance == null)
                {
                    _instance = _type.Field("<>4__this", FieldAttributes.Public, _importation[method.DeclaringType]);
                    var _variable = method.Body.Variables.Single(_Variable => _Variable.VariableType.Resolve() == _type);
                    method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Stfld, _genericity.Length > 0 ? new FieldReference(_instance.Name, method.DeclaringType, _type.MakeGenericType(_genericity)) : _instance));
                    method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldarg_0));
                    method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldloc, _variable));
                }
                _instance = (_instance as FieldDefinition).Relative();
            }
            var _state = _type.Fields.Single(_Field => _Field.Name == "<>1__state").Relative();
            var _builder = _type.Fields.Single(_Field => _Field.Name == "<>t__builder").Relative();
            var _offset = 0;
            var _begin = _move.Body.Instructions[_offset];
            var _resume = Instruction.Create(OpCodes.Ldarg_0);
            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _state));
            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldc_I4_0));
            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Bge, _resume));
            if (_instance != null)
            {
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _advice.Relative()));
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _instance));
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IAdvice>.Method(_Advice => _Advice.Instance<object>(Metadata<object>.Value)).GetGenericMethodDefinition()).MakeGenericMethod(_instance.FieldType))));
            }
            foreach (var _parameter in method.Parameters)
            {
                var _field = null as FieldReference;
                _field = _type.Fields.SingleOrDefault(_Field => _Field.Name == _parameter.Name);
                if (_field == null)
                {
                    _field = _type.Field(_parameter.Name, FieldAttributes.Public, _importation[_parameter.ParameterType]);
                    var _variable = method.Body.Variables.Single(_Variable => _Variable.VariableType.Resolve() == _type);
                    method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Stfld, _genericity.Length > 0 ? new FieldReference(_field.Name, _parameter.ParameterType, _type.MakeGenericType(_genericity)) : _field));
                    method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldarg, _parameter));
                    method.Body.Instructions.Insert(0, Instruction.Create(OpCodes.Ldloc, _variable));
                }
                _field = (_field as FieldDefinition).Relative();
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _advice.Relative()));
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldflda, _field));
                _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IAdvice>.Method(_Advice => _Advice.Argument<object>(ref Metadata<object>.Value)).GetGenericMethodDefinition()).MakeGenericMethod(_field.FieldType))));
            }
            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _advice.Relative()));
            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Callvirt, _move.Module.Import(Metadata<IAdvice>.Method(_Advice => _Advice.Begin()))));
            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Br_S, _begin));
            _move.Body.Instructions.Insert(_offset++, _resume);
            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _advice.Relative()));
            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Callvirt, _move.Module.Import(Metadata<IAdvice>.Method(_Advice => _Advice.Continue()))));
            while (_offset < _move.Body.Instructions.Count)
            {
                var _instruction = _move.Body.Instructions[_offset];
                if (_instruction.OpCode == OpCodes.Callvirt)
                {
                    if (_instruction.Operand is MethodReference)
                    {
                        var _operand = _instruction.Operand as MethodReference;
                        if (_operand.Name == "GetAwaiter")
                        {
                            var _action = _move.Body.Instructions[_offset - 1].Operand as MethodReference;
                            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Stloc, _task));
                            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
                            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _advice.Relative()));
                            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldtoken, _action));
                            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldtoken, _action.DeclaringType));
                            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Call, _move.Module.Import(Metadata.Method(() => MethodInfo.GetMethodFromHandle(Metadata<RuntimeMethodHandle>.Value, Metadata<RuntimeTypeHandle>.Value))))); //TODO Virtuoze => cache it!
                            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldloc, _task));
                            if (_action.ReturnType.Resolve() == _move.Module.Import(typeof(Task)).Resolve()) { _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Callvirt, _move.Module.Import(Metadata<IAdvice>.Method(_Advice => _Advice.Await(Metadata<MethodInfo>.Value, Metadata<Task>.Value))))); }
                            else { _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Callvirt, _move.Module.Import(Metadata<IAdvice>.Method(_Advice => _Advice.Await(Metadata<MethodInfo>.Value, Metadata<Task<object>>.Value)).GetGenericMethodDefinition()).MakeGenericMethod((_action.ReturnType as GenericInstanceType).GenericArguments[0]))); }
                            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldloc, _task));
                        }
                    }
                }
                else if (_instruction.OpCode == OpCodes.Call)
                {
                    if (_instruction.Operand is MethodReference)
                    {
                        var _operand = _instruction.Operand as MethodReference;
                        if (_operand.Name == "get_IsCompleted")
                        {
                            var _continue = _move.Body.Instructions[++_offset];
                            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Dup));
                            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Brfalse_S, _continue));
                            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldarg_0));
                            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Ldfld, _advice.Relative()));
                            _move.Body.Instructions.Insert(_offset++, Instruction.Create(OpCodes.Callvirt, _move.Module.Import(Metadata<IAdvice>.Method(_Advice => _Advice.Continue()))));
                        }
                        else if (_operand.Name == "SetResult")
                        {
                            var _return = _type.Method("<Return>", MethodAttributes.Public);
                            if (_operand.HasParameters)
                            {
                                var _parameter = new ParameterDefinition("<Value>", ParameterAttributes.None, (_builder.FieldType as GenericInstanceType).GenericArguments[0]);
                                _return.Parameters.Add(_parameter);
                                var _exception = _return.Body.Variable<Exception>();
                                var _disposed = _return.Body.Variable<bool>();
                                var _end = Instruction.Create(OpCodes.Ret);
                                _return.Body.Emit(OpCodes.Ldarg_0);
                                _return.Body.Emit(OpCodes.Ldfld, _advice.Relative());
                                _return.Body.Emit(OpCodes.Ldarga_S, _parameter);
                                _return.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IAdvice>.Method(_Advice => _Advice.Return(ref Metadata<object>.Value)).GetGenericMethodDefinition()).MakeGenericMethod(_parameter.ParameterType)));
                                _return.Body.Emit(OpCodes.Ldc_I4_1);
                                _return.Body.Emit(OpCodes.Stloc_1);
                                _return.Body.Emit(OpCodes.Ldarg_0);
                                _return.Body.Emit(OpCodes.Ldfld, _advice.Relative());
                                _return.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()))));
                                _return.Body.Emit(OpCodes.Ldarg_0);
                                _return.Body.Emit(OpCodes.Ldflda, _builder);
                                _return.Body.Emit(OpCodes.Ldarg_1);
                                _return.Body.Emit(OpCodes.Call, _operand);
                                _return.Body.Emit(OpCodes.Ret);
                                var _catch = _return.Body.Emit(OpCodes.Stloc_0);
                                _return.Body.Emit(OpCodes.Ldloc_1);
                                using (_return.Body.False())
                                {
                                    _return.Body.Emit(OpCodes.Ldarg_0);
                                    _return.Body.Emit(OpCodes.Ldfld, _advice.Relative());
                                    _return.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()))));
                                }
                                _return.Body.Emit(OpCodes.Ldarg_0);
                                _return.Body.Emit(OpCodes.Ldflda, _builder);
                                _return.Body.Emit(OpCodes.Ldloc_0);
                                var _method = _move.Module.Import(_builder.FieldType.Resolve().Methods.Single(_Method => _Method.Name == "SetException"));
                                _method.DeclaringType = _builder.FieldType;
                                _return.Body.Emit(OpCodes.Call, _method);
                                _return.Body.Emit(OpCodes.Ret);
                                _return.Body.Emit(OpCodes.Ret);
                                _return.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
                                {
                                    TryStart = _return.Body.Instructions[0],
                                    TryEnd = _return.Body.Instructions[_catch],
                                    HandlerStart = _return.Body.Instructions[_catch],
                                    HandlerEnd = _return.Body.Instructions[_return.Body.Instructions.Count - 1],
                                    CatchType = _exception.VariableType
                                });
                                _return.Body.OptimizeMacros();
                                _instruction.Operand = _type.HasGenericParameters ? _return.MakeHostInstanceGeneric(_type.GenericParameters.ToArray()) : _return;
                                _move.Body.Instructions[_offset - 2].OpCode = OpCodes.Nop;
                            }
                            else
                            {
                                var _exception = _return.Body.Variable<Exception>();
                                var _disposed = _return.Body.Variable<bool>();
                                var _end = Instruction.Create(OpCodes.Ret);
                                _return.Body.Emit(OpCodes.Ldarg_0);
                                _return.Body.Emit(OpCodes.Ldfld, _advice.Relative());
                                _return.Body.Emit(OpCodes.Callvirt, _move.Module.Import(Metadata<IAdvice>.Method(_Advice => _Advice.Return())));
                                _return.Body.Emit(OpCodes.Ldc_I4_1);
                                _return.Body.Emit(OpCodes.Stloc_1);
                                _return.Body.Emit(OpCodes.Ldarg_0);
                                _return.Body.Emit(OpCodes.Ldfld, _advice.Relative());
                                _return.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()))));
                                _return.Body.Emit(OpCodes.Ldarg_0);
                                _return.Body.Emit(OpCodes.Ldflda, _builder);
                                _return.Body.Emit(OpCodes.Call, _operand);
                                _return.Body.Emit(OpCodes.Ret);
                                var _catch = _return.Body.Emit(OpCodes.Stloc_0);
                                _return.Body.Emit(OpCodes.Ldloc_1);
                                using (_return.Body.False())
                                {
                                    _return.Body.Emit(OpCodes.Ldarg_0);
                                    _return.Body.Emit(OpCodes.Ldfld, _advice.Relative());
                                    _return.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()))));
                                }
                                _return.Body.Emit(OpCodes.Ldarg_0);
                                _return.Body.Emit(OpCodes.Ldflda, _builder);
                                _return.Body.Emit(OpCodes.Ldloc_0);
                                var _method = _move.Module.Import(_builder.FieldType.Resolve().Methods.Single(_Method => _Method.Name == "SetException"));
                                _method.DeclaringType = _builder.FieldType;
                                _return.Body.Emit(OpCodes.Call, _method);
                                _return.Body.Emit(OpCodes.Ret);
                                _return.Body.Emit(OpCodes.Ret);
                                _return.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
                                {
                                    TryStart = _return.Body.Instructions[0],
                                    TryEnd = _return.Body.Instructions[_catch],
                                    HandlerStart = _return.Body.Instructions[_catch],
                                    HandlerEnd = _return.Body.Instructions[_return.Body.Instructions.Count - 1],
                                    CatchType = _exception.VariableType,
                                });
                                _return.Body.OptimizeMacros();
                                _instruction.Operand = _type.HasGenericParameters ? _return.MakeHostInstanceGeneric(_type.GenericParameters.ToArray()) : _return;
                                _move.Body.Instructions[_offset - 1].OpCode = OpCodes.Nop;
                            }
                        }
                        else if (_operand.Name == "SetException")
                        {
                            var _throw = _type.Method("<Throw>", MethodAttributes.Public);
                            var _parameter = new ParameterDefinition("<Exception>", ParameterAttributes.None, _throw.Module.Import(typeof(Exception)));
                            _throw.Parameters.Add(_parameter);
                            if (_builder.FieldType.IsGenericInstance)
                            {
                                var _value = new VariableDefinition((_builder.FieldType as GenericInstanceType).GenericArguments[0]);
                                _throw.Body.Variables.Add(_value);
                                var _disposed = _throw.Body.Variable<bool>();
                                _throw.Body.Emit(OpCodes.Ldarg_0);
                                _throw.Body.Emit(OpCodes.Ldfld, _advice.Relative());
                                _throw.Body.Emit(OpCodes.Ldarg_S, _parameter);
                                _throw.Body.Emit(OpCodes.Ldloca_S, _value);
                                _throw.Body.Emit(OpCodes.Callvirt, _move.Module.Import(Metadata<IAdvice>.Method(_Advice => _Advice.Throw(ref Metadata<Exception>.Value, ref Metadata<object>.Value)).GetGenericMethodDefinition()).MakeGenericMethod(_value.VariableType));
                                _throw.Body.Emit(OpCodes.Ldc_I4_1);
                                _throw.Body.Emit(OpCodes.Stloc_1);
                                _throw.Body.Emit(OpCodes.Ldarg_0);
                                _throw.Body.Emit(OpCodes.Ldfld, _advice.Relative());
                                _throw.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()))));
                                _throw.Body.Emit(OpCodes.Ldarg_1);
                                using (_throw.Body.True())
                                {
                                    _throw.Body.Emit(OpCodes.Ldarg_0);
                                    _throw.Body.Emit(OpCodes.Ldflda, _builder);
                                    _throw.Body.Emit(OpCodes.Ldarg_1);
                                    _throw.Body.Emit(OpCodes.Call, _operand);
                                    _throw.Body.Emit(OpCodes.Ret);
                                }
                                _throw.Body.Emit(OpCodes.Ldarg_0);
                                _throw.Body.Emit(OpCodes.Ldflda, _builder);
                                _throw.Body.Emit(OpCodes.Ldloc_0);
                                var _method = _move.Module.Import(_move.Module.Import(_builder.FieldType.Resolve().Methods.Single(_Method => _Method.Name == "SetResult" && _Method.Parameters[0].ParameterType.IsGenericParameter)));
                                _method.DeclaringType = _builder.FieldType;
                                _throw.Body.Emit(OpCodes.Call, _method);
                                _throw.Body.Emit(OpCodes.Ret);
                                var _catch = _throw.Body.Emit(OpCodes.Starg, _parameter);
                                _throw.Body.Emit(OpCodes.Ldloc_1);
                                using (_throw.Body.False())
                                {
                                    _throw.Body.Emit(OpCodes.Ldarg_0);
                                    _throw.Body.Emit(OpCodes.Ldfld, _advice.Relative());
                                    _throw.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()))));
                                }
                                _throw.Body.Emit(OpCodes.Ldarg_0);
                                _throw.Body.Emit(OpCodes.Ldflda, _builder);
                                _throw.Body.Emit(OpCodes.Ldarg_1);
                                _throw.Body.Emit(OpCodes.Call, _operand);
                                _throw.Body.Emit(OpCodes.Ret);
                                _throw.Body.Emit(OpCodes.Ret);
                                _throw.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
                                {
                                    TryStart = _throw.Body.Instructions[0],
                                    TryEnd = _throw.Body.Instructions[_catch],
                                    HandlerStart = _throw.Body.Instructions[_catch],
                                    HandlerEnd = _throw.Body.Instructions[_throw.Body.Instructions.Count - 1],
                                    CatchType = _parameter.ParameterType,
                                });
                            }
                            else
                            {
                                var _disposed = _throw.Body.Variable<bool>();
                                _throw.Body.Emit(OpCodes.Ldarg_0);
                                _throw.Body.Emit(OpCodes.Ldfld, _advice.Relative());
                                _throw.Body.Emit(OpCodes.Ldarga_S, _parameter);
                                _throw.Body.Emit(OpCodes.Callvirt, _move.Module.Import(Metadata<IAdvice>.Method(_Advice => _Advice.Throw(ref Metadata<Exception>.Value))));
                                _throw.Body.Emit(OpCodes.Ldc_I4_1);
                                _throw.Body.Emit(OpCodes.Stloc_0);
                                _throw.Body.Emit(OpCodes.Ldarg_0);
                                _throw.Body.Emit(OpCodes.Ldfld, _advice.Relative());
                                _throw.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()))));
                                _throw.Body.Emit(OpCodes.Ldarg_1);
                                using (_throw.Body.True())
                                {
                                    _throw.Body.Emit(OpCodes.Ldarg_0);
                                    _throw.Body.Emit(OpCodes.Ldflda, _builder);
                                    _throw.Body.Emit(OpCodes.Ldarg_1);
                                    _throw.Body.Emit(OpCodes.Call, _operand);
                                    _throw.Body.Emit(OpCodes.Ret);
                                }
                                _throw.Body.Emit(OpCodes.Ldarg_0);
                                _throw.Body.Emit(OpCodes.Ldflda, _builder);
                                var _method = _move.Module.Import(_builder.FieldType.Resolve().Methods.Single(_Method => _Method.Name == "SetResult" && _Method.Parameters.Count == 0));
                                _method.DeclaringType = _builder.FieldType;
                                _throw.Body.Emit(OpCodes.Call, _method);
                                _throw.Body.Emit(OpCodes.Ret);
                                var _catch = _throw.Body.Emit(OpCodes.Starg, _parameter);
                                _throw.Body.Emit(OpCodes.Ldloc_0);
                                using (_throw.Body.False())
                                {
                                    _throw.Body.Emit(OpCodes.Ldarg_0);
                                    _throw.Body.Emit(OpCodes.Ldfld, _advice.Relative());
                                    _throw.Body.Emit(OpCodes.Callvirt, _move.Module.Import(_move.Module.Import(Metadata<IDisposable>.Method(_IDisposable => _IDisposable.Dispose()))));
                                }
                                _throw.Body.Emit(OpCodes.Ldarg_0);
                                _throw.Body.Emit(OpCodes.Ldflda, _builder);
                                _throw.Body.Emit(OpCodes.Ldarg_1);
                                _throw.Body.Emit(OpCodes.Call, _operand);
                                _throw.Body.Emit(OpCodes.Ret);
                                _throw.Body.Emit(OpCodes.Ret);
                                _throw.Body.ExceptionHandlers.Add(new ExceptionHandler(ExceptionHandlerType.Catch)
                                {
                                    TryStart = _throw.Body.Instructions[0],
                                    TryEnd = _throw.Body.Instructions[_catch],
                                    HandlerStart = _throw.Body.Instructions[_catch],
                                    HandlerEnd = _throw.Body.Instructions[_throw.Body.Instructions.Count - 1],
                                    CatchType = _parameter.ParameterType,
                                });
                            }
                            _throw.Body.OptimizeMacros();
                            _instruction.Operand = _type.HasGenericParameters ? _throw.MakeHostInstanceGeneric(_type.GenericParameters.ToArray()) : _throw;
                            _move.Body.Instructions[_offset - 2].OpCode = OpCodes.Nop;
                        }
                    }
                }
                _offset++;
            }
            _move.Body.OptimizeMacros();
        }
    }
}

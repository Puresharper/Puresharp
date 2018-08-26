using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
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
    public class Intermediate
    {
        static private readonly MethodInfo GetMethodHandle = Metadata<MethodBase>.Property(_Method => _Method.MethodHandle).GetGetMethod();
        static private readonly MethodInfo GetFunctionPointer = Metadata<RuntimeMethodHandle>.Method(_Method => _Method.GetFunctionPointer());

        private TypeDefinition m_Type;
        private Authentic m_Authentic;
        private Dictionary<MethodDefinition, FieldDefinition> m_Dictionary;

        public Intermediate(TypeDefinition type, Authentic authentic)
        {
            this.m_Type = type.Type("<Intermediate>", TypeAttributes.NestedPublic | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract);
            foreach (var _parameter in type.GenericParameters) { this.m_Type.GenericParameters.Add(_parameter.Copy(this.m_Type)); }
            this.m_Authentic = authentic;
            this.m_Dictionary = new Dictionary<MethodDefinition, FieldDefinition>();
            foreach (var _method in type.DeclaringType.Methods.ToArray())
            {
                if (Bypass.Match(_method)) { continue; }
                this.m_Dictionary.Add(_method, this.Manage(_method));
            }
        }

        private FieldDefinition Manage(MethodDefinition method)
        {
            var _type = this.m_Type.Type(method.IsConstructor ? "<<Constructor>>" : $"<{ method.Name }>", TypeAttributes.NestedPublic | TypeAttributes.Abstract | TypeAttributes.Sealed);
            foreach (var _parameter in this.m_Type.GenericParameters) { _type.GenericParameters.Add(_parameter.Copy(_type)); }
            foreach (var _parameter in method.GenericParameters) { _type.GenericParameters.Add(_parameter.Copy(_type)); }
            var _handle = _type.Field<object>("<Handle>", FieldAttributes.Static | FieldAttributes.Public).Relative();
            var _authentic = _type.Field<IntPtr>("<Authentic>", FieldAttributes.Static | FieldAttributes.Public).Relative();
            var _auxiliary = _type.Field<IntPtr>("<Auxiliary>", FieldAttributes.Static | FieldAttributes.Public).Relative();
            var _field = _type.Field<IntPtr>("<Pointer>", FieldAttributes.Static | FieldAttributes.Public);
            var _pointer = _field.Relative();
            var _update = _type.Method("<Update>", MethodAttributes.Static | MethodAttributes.Private);
            _update.Parameter<IntPtr>("<Pointer>");
            var _return = Instruction.Create(OpCodes.Ret);
            using (_update.Body.Lock(_handle))
            {
                _update.Body.Emit(OpCodes.Ldsfld, _auxiliary);
                using (_update.Body.True())
                {
                    _update.Body.Emit(OpCodes.Ldarg_0);
                    _update.Body.Emit(OpCodes.Stsfld, _auxiliary);
                    _update.Body.Emit(OpCodes.Leave, _return);
                }
                _update.Body.Emit(OpCodes.Ldarg_0);
                _update.Body.Emit(OpCodes.Stsfld, _pointer);
            }
            _update.Body.Emit(_return);
            var _primary = _type.Method(method.IsConstructor ? "<Constructor>" : method.Name, MethodAttributes.Static | MethodAttributes.Private);
            _primary.ReturnType = _primary.Resolve(method.ReturnType);
            if (!method.IsStatic) { _primary.Parameter("this", ParameterAttributes.None, _primary.Resolve(method.DeclaringType)); }
            foreach (var _parameter in method.Parameters) { _primary.Parameter(_parameter.Name, _parameter.Attributes, _primary.Resolve(_parameter.ParameterType)); }
            _primary.Body.Variable<IntPtr>("<Pointer>");
            _primary.Body.Emit(OpCodes.Ldsfld, _handle);
            _primary.Body.Emit(OpCodes.Call, Metadata.Method(() => Monitor.Enter(Metadata<object>.Value)));

            _primary.Body.Emit(Metadata<Action<MethodBase>>.Type);
            _primary.Body.Emit(typeof(Metadata));
            _primary.Body.Emit(OpCodes.Ldstr, "Broadcast");
            _primary.Body.Emit(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
            _primary.Body.Emit(OpCodes.Call, Metadata<Type>.Method(_Type => _Type.GetMethod(Metadata<string>.Value, Metadata<BindingFlags>.Value)));
            _primary.Body.Emit(OpCodes.Call, Metadata.Method(() => Delegate.CreateDelegate(Metadata<Type>.Value, Metadata<MethodInfo>.Value)));

            var _importation = new Importation(method, _type);

            //_primary.Body.Emit(_importation[method]);


            //TODO THONGTO
            _primary.Body.Emit(method);
            if (method.GenericParameters.Count > 0)
            {
                _primary.Body.Emit(OpCodes.Ldc_I4, method.GenericParameters.Count);
                _primary.Body.Emit(OpCodes.Newarr, typeof(Type));
                for (var i = 0; i < method.GenericParameters.Count; i++)
                {
                    _primary.Body.Emit(OpCodes.Dup);
                    _primary.Body.Emit(OpCodes.Ldc_I4, i);
                    _primary.Body.Emit(_type.GenericParameters.Skip(method.DeclaringType.GenericParameters.Count).ElementAt(i));
                    _primary.Body.Emit(OpCodes.Stelem_Ref);
                }
                _primary.Body.Emit(OpCodes.Callvirt, Metadata<MethodInfo>.Method(_Method => _Method.MakeGenericMethod(Metadata<Type[]>.Value)));
            }

            _primary.Body.Emit(OpCodes.Callvirt, Metadata<Action<MethodBase>>.Method(_Action => _Action.Invoke(Metadata<MethodBase>.Value)));

            _primary.Body.Emit(OpCodes.Ldsfld, _pointer);
            _primary.Body.Emit(OpCodes.Stloc_0);
            _primary.Body.Emit(OpCodes.Ldsfld, _auxiliary);
            using (_primary.Body.True())
            {
                _primary.Body.Emit(OpCodes.Ldsfld, _auxiliary);
                _primary.Body.Emit(OpCodes.Stloc_0);
                _primary.Body.Emit(OpCodes.Ldloc_0);
                _primary.Body.Emit(OpCodes.Stsfld, _pointer);
                _primary.Body.Emit(OpCodes.Ldsfld, Metadata.Field(() => IntPtr.Zero));
                _primary.Body.Emit(OpCodes.Stsfld, _auxiliary);
            }
            _primary.Body.Emit(OpCodes.Ldsfld, _handle);
            _primary.Body.Emit(OpCodes.Call, Metadata.Method(() => Monitor.Exit(Metadata<object>.Value)));
            for (var _index = 0; _index < _primary.Parameters.Count; _index++)
            {
                switch (_index)
                {
                    case 0: _primary.Body.Emit(OpCodes.Ldarg_0); break;
                    case 1: _primary.Body.Emit(OpCodes.Ldarg_1); break;
                    case 2: _primary.Body.Emit(OpCodes.Ldarg_2); break;
                    case 3: _primary.Body.Emit(OpCodes.Ldarg_3); break;
                    default: _primary.Body.Emit(OpCodes.Ldarg_S, _primary.Parameters[_index]); break;
                }
            }
            _primary.Body.Emit(OpCodes.Ldloc_0);
            _primary.Body.Emit(OpCodes.Calli, _primary.ReturnType, _primary.Parameters);
            _primary.Body.Emit(OpCodes.Ret);
            _primary.Body.OptimizeMacros();
            var _initializer = _type.Initializer();
            var _variable = _initializer.Body.Variable<RuntimeMethodHandle>();
            _initializer.Body.Variable<Func<IntPtr>>();
            _initializer.Body.Emit(OpCodes.Newobj, Metadata.Constructor(() => new object()));
            _initializer.Body.Emit(OpCodes.Stsfld, _handle);
            //var met = new MethodReference(this.m_Authentic[method].Name, _primary.ReturnType, this.m_Authentic[method].DeclaringType.MakeGenericType(_type.GenericParameters.Take(this.m_Type.GenericParameters.Count)));
            //foreach (var p in met.GenericParameters) { met.GenericParameters.Add(new GenericParameter(p.Name, met)); }
            //foreach (var _parameter in this.m_Authentic[method].Parameters) { met.Parameters.Add(new ParameterDefinition(_parameter.Name, _parameter.Attributes, _parameter.ParameterType.IsMethodGenericParameterType() ? _type.GenericParameterType(_parameter.ParameterType.Name) : _parameter.ParameterType)); }
            //_initializer.Body.Emit(met.MakeGenericMethod(_type.GenericParameters.Reverse().Take(method.GenericParameters.Count).Reverse().ToArray()));
            _initializer.Body.Emit(_importation[this.m_Authentic[method]]);
            _initializer.Body.Emit(OpCodes.Callvirt, Intermediate.GetMethodHandle);
            _initializer.Body.Emit(OpCodes.Stloc_0);
            _initializer.Body.Emit(OpCodes.Ldloca_S, _variable);
            _initializer.Body.Emit(OpCodes.Callvirt, Intermediate.GetFunctionPointer);
            _initializer.Body.Emit(OpCodes.Stsfld, _authentic);
            _initializer.Body.Emit(OpCodes.Ldsfld, _authentic);
            _initializer.Body.Emit(OpCodes.Stsfld, _auxiliary);
            //var met2 = new MethodReference(_primary.Name, _primary.ReturnType, _type.MakeGenericType(_type.GenericParameters));
            //foreach (var _parameter in _primary.Parameters) { met2.Parameters.Add(new ParameterDefinition(_parameter.Name, _parameter.Attributes, _parameter.ParameterType.IsMethodGenericParameterType() ? _type.GenericParameterType(_parameter.ParameterType.Name) : _parameter.ParameterType)); }
            //_initializer.Body.Emit(met2);
            //_initializer.Body.Emit(new MethodReference(_primary.Name, _primary.ReturnType, this.m_Authentic[method].DeclaringType.MakeGenericType(_type.GenericParameters)));
            _initializer.Body.Emit(_importation[_primary]);
            _initializer.Body.Emit(OpCodes.Callvirt, Intermediate.GetMethodHandle);
            _initializer.Body.Emit(OpCodes.Stloc_0);
            _initializer.Body.Emit(OpCodes.Ldloca_S, _variable);
            _initializer.Body.Emit(OpCodes.Callvirt, Intermediate.GetFunctionPointer);
            _initializer.Body.Emit(OpCodes.Stsfld, _pointer);
            _initializer.Body.Emit(OpCodes.Ret);
            _initializer.Body.OptimizeMacros();
            return _field;
        }

        public TypeDefinition Type
        {
            get { return this.m_Type; }
        }

        public FieldDefinition this[MethodDefinition method]
        {
            get { return this.m_Dictionary[method]; }
        }
    }
}

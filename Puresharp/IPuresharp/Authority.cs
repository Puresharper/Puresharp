using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
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
    public class Authority
    {
        static private readonly List<AssemblyDefinition> m_Inclusion = new List<AssemblyDefinition>();

        static private IEnumerable<ModuleDefinition> Modules(AssemblyDefinition assembly)
        {
            var _hashset = new HashSet<ModuleDefinition>();
            foreach (var _reference in assembly.MainModule.AssemblyReferences)
            {
                if (_reference.Name == "mscorlib" || _reference.Name == "System" || _reference.Name.StartsWith("System.") || _reference.Name == "Puresharp" || _reference.Name.StartsWith("Puresharp.")) { continue; }
                foreach (var _module in Authority.Modules(assembly.MainModule.AssemblyResolver.Resolve(_reference)))
                {
                    _hashset.Add(_module);
                }
            }
            _hashset.Add(assembly.MainModule);
            return _hashset;
        }

        static private void Include(AssemblyDefinition assembly, params Assembly[] dependencies)
        {
            if (Authority.m_Inclusion.Contains(assembly)) { return; }
            var _module = assembly.MainModule.Types.First(_Type => _Type.Name == Program.Module);
            var _initializer = _module.Initializer();
            var _fake = _module.Method<Assembly>("<Puresharp<Fake>>", MethodAttributes.Static | MethodAttributes.Private);
            _fake.Parameter<object>("sender");
            _fake.Parameter<ResolveEventArgs>("arguments");
            foreach (var _dependency in dependencies)
            {
                var _lazy = _module.Field<Lazy<Assembly>>($"<{ _dependency.GetName().Name }>", FieldAttributes.Static | FieldAttributes.Private);
                Authority.m_Inclusion.Add(assembly);
                var _make = _module.Method<Assembly>($"<{ _dependency.GetName().Name }<Make>>", MethodAttributes.Static | MethodAttributes.Private);
                _make.Body.Emit(OpCodes.Ldstr, Convert.ToBase64String(File.ReadAllBytes(_dependency.Location)));
                _make.Body.Emit(OpCodes.Call, Metadata.Method(() => Convert.FromBase64String(Metadata<string>.Value)));
                _make.Body.Emit(OpCodes.Call, Metadata.Method(() => Assembly.Load(Metadata<byte[]>.Value)));
                _make.Body.Emit(OpCodes.Ret);
                _fake.Body.Emit(OpCodes.Ldarg_1);
                _fake.Body.Emit(OpCodes.Call, Metadata<ResolveEventArgs>.Property(_ResolveEventArgs => _ResolveEventArgs.Name).GetGetMethod());
                _fake.Body.Emit(OpCodes.Ldstr, _dependency.FullName);
                _fake.Body.Emit(OpCodes.Call, Metadata.Method(() => string.Equals(Metadata<string>.Value, Metadata<string>.Value)));
                using (_fake.Body.True())
                {
                    _fake.Body.Emit(OpCodes.Ldsfld, _lazy);
                    _fake.Body.Emit(OpCodes.Call, Metadata<Lazy<Assembly>>.Property(_Lazy => _Lazy.Value).GetGetMethod());
                    _fake.Body.Emit(OpCodes.Ret);
                }
                _initializer.Body.Emit(OpCodes.Ldnull);
                _initializer.Body.Emit(OpCodes.Ldftn, _make);
                _initializer.Body.Emit(OpCodes.Newobj, Metadata<Func<Assembly>>.Type.GetConstructors().Single());
                _initializer.Body.Emit(OpCodes.Ldc_I4, (int)LazyThreadSafetyMode.ExecutionAndPublication);
                _initializer.Body.Emit(OpCodes.Newobj, Metadata.Constructor(() => new Lazy<Assembly>(Metadata<Func<Assembly>>.Value, Metadata<LazyThreadSafetyMode>.Value)));
                _initializer.Body.Emit(OpCodes.Stsfld, _lazy);
            }
            _fake.Body.Emit(OpCodes.Ldnull);
            _fake.Body.Emit(OpCodes.Ret);
            _initializer.Body.Emit(OpCodes.Call, Metadata.Property(() => AppDomain.CurrentDomain).GetGetMethod());
            _initializer.Body.Emit(OpCodes.Ldnull);
            _initializer.Body.Emit(OpCodes.Ldftn, _fake);
            _initializer.Body.Emit(OpCodes.Newobj, typeof(ResolveEventHandler).GetConstructors().Single());
            _initializer.Body.Emit(OpCodes.Callvirt, typeof(AppDomain).GetMethod("add_AssemblyResolve", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

            foreach (var _r in Authority.Modules(assembly))
            {
                foreach (var _type in _r.GetTypes())
                {
                    if (_type.Name.StartsWith("<")) { continue; }
                    foreach (var _method in _type.Methods)
                    {
                        if (_method.IsStatic && _method.Parameters.Count == 0 && _method.CustomAttributes.Any(_Attribute => _Attribute.AttributeType.Name == "Startup") && _method.ReturnType.Resolve() == _method.Module.Import(typeof(void)).Resolve())
                        {
                            _initializer.Body.Emit(Metadata<Action<MethodInfo>>.Type);
                            _initializer.Body.Emit(Metadata<Startup>.Type);
                            _initializer.Body.Emit(OpCodes.Ldstr, "Run");
                            _initializer.Body.Emit(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
                            _initializer.Body.Emit(OpCodes.Call, Metadata<Type>.Method(_Type => _Type.GetMethod(Metadata<string>.Value, Metadata<BindingFlags>.Value)));
                            _initializer.Body.Emit(OpCodes.Call, Metadata.Method(() => Delegate.CreateDelegate(Metadata<Type>.Value, Metadata<MethodInfo>.Value)));
                            _initializer.Body.Emit(assembly.MainModule.Import(_method));
                            _initializer.Body.Emit(OpCodes.Callvirt, Metadata<Action<MethodInfo>>.Method(_Action => _Action.Invoke(Metadata<MethodInfo>.Value)));
                        }
                    }
                }
            }
            _initializer.Body.Emit(OpCodes.Ret);
            _initializer.Body.OptimizeMacros();
        }

        private TypeDefinition m_Type;
        private Authentic m_Authentic;
        private Intermediate m_Intermediate;

        public Authority(TypeDefinition type)
        {
            this.m_Type = type.Type("<Puresharp>", TypeAttributes.Class | TypeAttributes.NestedPrivate | TypeAttributes.Sealed | TypeAttributes.Abstract);
            foreach (var _parameter in type.GenericParameters) { this.m_Type.GenericParameters.Add(_parameter.Copy(this.m_Type)); }
            foreach (var _method in type.Methods.ToArray())
            {
                if (Bypass.Match(_method)) { continue; }
                Machine.Manage(_method);
            }
            this.m_Authentic = new Authentic(this.m_Type);
            this.m_Intermediate = new Intermediate(this.m_Type, this.m_Authentic);
            foreach (var _method in type.Methods.ToArray())
            {
                if (Bypass.Match(_method)) { continue; }
                this.Manage(type, _method);
            }
        }

        private void Manage(TypeDefinition type, MethodDefinition method)
        {
            Authority.Include(method.Module.Assembly, typeof(IAdvice).Assembly);
            var _authentic = this.m_Authentic[method];
            var _intermediate = this.m_Intermediate[method];
            method.Body = new MethodBody(method);
            for (var _index = 0; _index < _authentic.Parameters.Count; _index++)
            {
                switch (_index)
                {
                    case 0: method.Body.Emit(OpCodes.Ldarg_0); break;
                    case 1: method.Body.Emit(OpCodes.Ldarg_1); break;
                    case 2: method.Body.Emit(OpCodes.Ldarg_2); break;
                    case 3: method.Body.Emit(OpCodes.Ldarg_3); break;
                    default: method.Body.Emit(OpCodes.Ldarg_S, method.Parameters[method.IsStatic ? _index : _index - 1]); break;
                }
            }
            method.Body.Emit(OpCodes.Ldsfld, new FieldReference(_intermediate.Name, _intermediate.FieldType, _intermediate.DeclaringType.MakeGenericType(method.DeclaringType.GenericParameters.Concat(method.GenericParameters))));
            method.Body.Emit(OpCodes.Calli, method.ReturnType, method.Body.Resolve(_authentic).Parameters);
            method.Body.Emit(OpCodes.Ret);
            method.Body.OptimizeMacros();
        }

        //private FieldDefinition Introspection(MethodDefinition method)
        //{
        //    var _type = this.m_Type.Type($"<<{method.Name}>>", TypeAttributes.NestedPublic | TypeAttributes.Class | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit);
        //    foreach (var _parameter in method.DeclaringType.GenericParameters) { _type.GenericParameters.Add(new GenericParameter(_parameter.Name, _type)); }
        //    foreach (var _parameter in method.GenericParameters) { _type.GenericParameters.Add(new GenericParameter(_parameter.Name, _type)); }
        //    var _method = _type.Field<MethodBase>("<Method>", FieldAttributes.Static | FieldAttributes.Public);
        //    var _signature = _type.Field<ParameterInfo[]>("<Signature>", FieldAttributes.Static | FieldAttributes.Public);
        //    var _initializer = _type.Initializer();
        //    //if (_metadata.HasGenericParameters)
        //    //{
        //    //    _initializer.Body.Emit(method.MakeGenericMethod(_metadata.GenericParameters)); //TODO bug!
        //    //    _initializer.Body.Emit(OpCodes.Stsfld, new FieldReference(_method.Name, _method.FieldType, _metadata.MakeGenericType(_metadata.GenericParameters)));
        //    //}
        //    //else
        //    //{
        //    _initializer.Body.Emit(method);
        //    if (method.GenericParameters.Count > 0)
        //    {
        //        _initializer.Body.Emit(OpCodes.Ldc_I4, method.GenericParameters.Count);
        //        _initializer.Body.Emit(OpCodes.Newarr, typeof(Type));
        //        for (var i = 0; i < method.GenericParameters.Count; i++)
        //        {
        //            _initializer.Body.Emit(OpCodes.Dup);
        //            _initializer.Body.Emit(OpCodes.Ldc_I4, i);
        //            _initializer.Body.Emit(_type.GenericParameters.Skip(method.DeclaringType.GenericParameters.Count).ElementAt(i));
        //            _initializer.Body.Emit(OpCodes.Stelem_Ref);
        //        }
        //        _initializer.Body.Emit(OpCodes.Callvirt, Metadata<MethodInfo>.Method(_Method => _Method.MakeGenericMethod(Metadata<Type[]>.Value)));
        //    }



        //    _initializer.Body.Emit(OpCodes.Stsfld, _method);
        //    //}
        //    _initializer.Body.Emit(OpCodes.Ldsfld, _method);
        //    _initializer.Body.Emit(OpCodes.Callvirt, Metadata<MethodBase>.Method(_Method => _Method.GetParameters()));
        //    _initializer.Body.Emit(OpCodes.Stsfld, _signature);
        //    for (var _index = 0; _index < method.Parameters.Count; _index++)
        //    {
        //        _initializer.Body.Emit(OpCodes.Ldsfld, _signature);
        //        _initializer.Body.Emit(OpCodes.Ldc_I4, _index);
        //        _initializer.Body.Emit(OpCodes.Ldelem_Ref);
        //        _initializer.Body.Emit(OpCodes.Stsfld, _type.Field<ParameterInfo>(method.Parameters[_index].Name, FieldAttributes.Static | FieldAttributes.Public));
        //    }
        //    _initializer.Body.Emit(OpCodes.Ret);
        //    _initializer.Body.OptimizeMacros();
        //    return _method;
        //}

        public TypeDefinition Type
        {
            get { return this.m_Type; }
        }

        public Authentic Authentic
        {
            get { return this.m_Authentic; }
        }

        public Intermediate Intermediate
        {
            get { return this.m_Intermediate; }
        }
    }
}

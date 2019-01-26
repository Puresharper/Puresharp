using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Puresharp;

using Assembly = System.Reflection.Assembly;
using MethodBase = System.Reflection.MethodBase;
using MethodInfo = System.Reflection.MethodInfo;
using ParameterInfo = System.Reflection.ParameterInfo;
using BindingFlags = System.Reflection.BindingFlags;
using CallSite = Mono.Cecil.CallSite;

namespace IPuresharp
{
    static public class Program
    {
        private const string Puresharp = "<Puresharp>";
        public const string Module = "<Module>";

        static private readonly MethodInfo GetMethodHandle = Metadata<MethodBase>.Property(_Method => _Method.MethodHandle).GetGetMethod();
        static private readonly MethodInfo GetFunctionPointer = Metadata<RuntimeMethodHandle>.Method(_Method => _Method.GetFunctionPointer());
        static private readonly MethodInfo CreateDelegate = Metadata.Method(() => Delegate.CreateDelegate(Metadata<Type>.Value, Metadata<MethodInfo>.Value));
        static private readonly List<AssemblyDefinition> m_Inclusion = new List<AssemblyDefinition>();

        static public void Main(string[] arguments)
        {
            var _directory = Environment.CurrentDirectory;
            try
            {
                Environment.CurrentDirectory = new Uri(Path.GetDirectoryName((typeof(Program).Assembly.CodeBase))).LocalPath;
                if (arguments == null) { throw new ArgumentNullException(); }
                switch (arguments.Length)
                {
                    case 1: Program.Manage(arguments[0]); break;
                    default: throw new ArgumentOutOfRangeException("IPuresharp.exe can only be used with an assembly full path as single argument.");
                }
            }
            catch { throw; }
            finally { Environment.CurrentDirectory = _directory; }
        }

        static private void Manage(string assembly)
        {
            var _directory = Path.GetDirectoryName(assembly);
            using (var _resolver = new DefaultAssemblyResolver())
            {
                _resolver.AddSearchDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"dotnet\shared"));
                _resolver.AddSearchDirectory(_directory);
                if (Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".nuget\packages"))) { _resolver.AddSearchDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), @".nuget\packages")); }
                using (var _assembly = AssemblyDefinition.ReadAssembly(assembly, new ReaderParameters() { AssemblyResolver = _resolver, ReadSymbols = true, ReadingMode = ReadingMode.Immediate, ReadWrite = true, InMemory = true }))
                {
                    var _module = _assembly.MainModule;
                    foreach (var _type in _module.GetTypes().ToArray()) { Program.Manage(_type); }
                    _assembly.Write(assembly, new WriterParameters { WriteSymbols = true });
                }
            }
        }
        
        static private void Manage(TypeDefinition type)
        {
            if (Bypass.Match(type)) { return; }
            new Authority(type);
        }
    }

    public sealed class AssemblyResolutionException : Exception
    {

        readonly AssemblyNameReference reference;

        public AssemblyNameReference AssemblyReference
        {
            get { return this.reference; }
        }

        public AssemblyResolutionException(AssemblyNameReference reference)
            : base(string.Format("Failed to resolve assembly: '{0}'", reference))
        {
            this.reference = reference;
        }
    }

    //public class UniversalAssemblyResolver : IAssemblyResolver
    //{
    //    private Dictionary<string, AssemblyDefinition> m_Dictionary;

    //    public UniversalAssemblyResolver()
    //    {
    //        this.m_Dictionary = new Dictionary<string, AssemblyDefinition>();
    //    }

    //    public void AddSearchDirectory(string directory)
    //    {
    //        foreach (var _filename in Directory.EnumerateFiles(new Uri(directory).LocalPath, "*.dll", SearchOption.AllDirectories).Concat(Directory.EnumerateFiles(new Uri(directory).LocalPath, "*.exe", SearchOption.AllDirectories)))
    //        {
    //            try
    //            {
    //                var _library = AssemblyDefinition.ReadAssembly(_filename, new ReaderParameters() { AssemblyResolver = this });
    //                var _name = (_library.Name.Name + _library.Name.Version.ToString()).ToLower();
    //                if (this.m_Dictionary.ContainsKey(_name))
    //                {
    //                    _library.Dispose();
    //                    continue;
    //                }
    //                this.m_Dictionary.Add(_name, _library);
    //            }
    //            catch { }
    //        }
    //    }

    //    public AssemblyDefinition Resolve(AssemblyNameReference name)
    //    {
    //        if (this.m_Dictionary.TryGetValue(name.Name, out var _lazy)) { return _lazy.Value; }
    //        _lazy = new Lazy<AssemblyDefinition>(() =>
    //        {
    //            var jjj = this.m_Exploration.SelectMany(_Directory => Directory.EnumerateFiles(new Uri(_Directory).LocalPath, "*.dll", SearchOption.AllDirectories)).Select(_Filename =>
    //            {
    //                var _library = null as AssemblyDefinition;
    //                try { _library = AssemblyDefinition.ReadAssembly(_Filename, new ReaderParameters() { AssemblyResolver = this }); } catch { }
    //                return new { Filename = _Filename, Library = _library };
    //            }).Where(_Assembly => _Assembly.Library != null && _Assembly.Library.Name.Name == name.Name).ToArray();
    //            var _value = jjj.FirstOrDefault();
    //            if (_value == null) { throw new NotSupportedException(); }
    //            return AssemblyDefinition.ReadAssembly(_value.Filename, new ReaderParameters() { AssemblyResolver = this });
    //        });
    //        var _assembly = _lazy.Value;
    //        this.m_Dictionary.Add(name.Name, _lazy);
    //        return _assembly;
    //    }

    //    public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
    //    {
    //        return this.Resolve(name);
    //    }

    //    public void Dispose()
    //    {
    //        foreach (var _lazy in this.m_Dictionary.Values)
    //        {
    //            if (!_lazy.IsValueCreated) { continue; }
    //            _lazy.Value.Dispose();
    //        }
    //    }
    //}
}

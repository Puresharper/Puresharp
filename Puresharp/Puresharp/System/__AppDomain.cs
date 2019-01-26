using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static internal class __AppDomain
    {
        static public ModuleBuilder DefineDynamicModule(this AppDomain domain)
        {
            return domain.DefineDynamicModule(Guid.NewGuid().ToString("N"));
        }

        static public ModuleBuilder DefineDynamicModule(this AppDomain domain, string name)
        {
            #if NET452
            return domain.DefineDynamicAssembly(new AssemblyName(string.Concat(Metadata<Assembly>.Type.Name, name)), AssemblyBuilderAccess.Run).DefineDynamicModule(string.Concat(Metadata<Module>.Type.Name, name), false);
            #else
            return AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(string.Concat(Metadata<Assembly>.Type.Name, name)), AssemblyBuilderAccess.Run).DefineDynamicModule(string.Concat(Metadata<Module>.Type.Name, name));
            #endif
        }

        static public ModuleBuilder DefineDynamicModule(this AppDomain domain, string name, string key)
        {
            var _name = new AssemblyName(name);
            _name.SetPublicKey(Enumerable.Range(0, key.Length).Where(_X => _X % 2 == 0).Select(_X => Convert.ToByte(key.Substring(_X, 2), 16)).ToArray());
            #if NET452
            return domain.DefineDynamicAssembly(_name, AssemblyBuilderAccess.Run).DefineDynamicModule(string.Concat(Metadata<Module>.Type.Name, name), false);
            #else
            return AssemblyBuilder.DefineDynamicAssembly(_name, AssemblyBuilderAccess.Run).DefineDynamicModule(string.Concat(Metadata<Module>.Type.Name, name));
            #endif
        }
    }
}

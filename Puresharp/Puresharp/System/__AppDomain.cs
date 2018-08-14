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
            return domain.DefineDynamicAssembly(new AssemblyName(string.Concat(Metadata<Assembly>.Type.Name, name)), AssemblyBuilderAccess.Run).DefineDynamicModule(string.Concat(Metadata<Module>.Type.Name, name), false);
        }

        static public ModuleBuilder DefineDynamicModule(this AppDomain domain, string name, string key)
        {
            var _name = new AssemblyName(name);
            _name.SetPublicKey(Enumerable.Range(0, key.Length).Where(_X => _X % 2 == 0).Select(_X => Convert.ToByte(key.Substring(_X, 2), 16)).ToArray());
            return domain.DefineDynamicAssembly(_name, AssemblyBuilderAccess.Run).DefineDynamicModule(string.Concat(Metadata<Module>.Type.Name, name), false);
        }
    }
}

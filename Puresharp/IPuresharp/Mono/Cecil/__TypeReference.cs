using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mono.Cecil
{
    static internal class __TypeReference
    {
        static public bool IsAssignableFrom(this TypeReference type, TypeDefinition speculative)
        {
            var _type = type.Resolve();
            if (speculative.BaseType != null && speculative.BaseType.Resolve() == _type) { return true; }
            if (speculative.Interfaces.Any(_Type => _Type.InterfaceType.Resolve() == _type)) { return true; }
            return false;
        }

        static public bool Is<T>(this TypeReference @this)
        {
            return @this.Is(@this.Module.Import(typeof(T)));
        }

        static public bool Is(this TypeReference @this, TypeReference type)
        {
            if (@this == null || type == null) { return false; }
            if (@this.FullName == type.FullName && @this.Module.Name == type.Module.Name) { return true; }
            return @this.Resolve().BaseType.Is(type) || @this.Resolve().Interfaces.Any(_Interface => _Interface.InterfaceType.Is(type));
        }
    }
}

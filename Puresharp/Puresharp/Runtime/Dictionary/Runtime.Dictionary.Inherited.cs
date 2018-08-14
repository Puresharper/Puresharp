using System;
using System.Linq;
using System.Reflection;

namespace Puresharp
{
    static internal partial class Runtime
    {
        static internal partial class Dictionary
        {
            static public class Inherited
            {
                static public readonly Data.Map<Type, Data.Collection<FieldInfo>> Fields = new Data.Map<Type, Data.Collection<FieldInfo>>(_Type => new Data.Collection<FieldInfo>(Runtime.Lookup.Fields(_Type).OrderBy(_Field => _Field.DeclaringType.MetadataToken).ThenBy(_Field => _Field.MetadataToken).ToArray()), Concurrency.Interlocked);
                static public readonly Data.Map<Type, Data.Collection<PropertyInfo>> Properties = new Data.Map<Type, Data.Collection<PropertyInfo>>(_Type => new Data.Collection<PropertyInfo>(Runtime.Lookup.Properties(_Type).OrderBy(_Property => _Property.DeclaringType.MetadataToken).ThenBy(_Property => _Property.MetadataToken).ToArray()), Concurrency.Interlocked);
                static public readonly Data.Map<Type, Data.Collection<MethodInfo>> Methods = new Data.Map<Type, Data.Collection<MethodInfo>>(_Type => new Data.Collection<MethodInfo>(Runtime.Lookup.Methods(_Type).OrderBy(_Method => _Method.DeclaringType.MetadataToken).ThenBy(_Method => _Method.MetadataToken).ToArray()), Concurrency.Interlocked);
            }
        }
    }

    static internal partial class Runtime<T>
    {
        static internal partial class Dictionary
        {
            static public class Inherited
            {
                static public readonly Data.Collection<FieldInfo> Fields = Runtime.Dictionary.Inherited.Fields[Metadata<T>.Type];
                static public readonly Data.Collection<PropertyInfo> Properties = Runtime.Dictionary.Inherited.Properties[Metadata<T>.Type];
                static public readonly Data.Collection<MethodInfo> Methods = Runtime.Dictionary.Inherited.Methods[Metadata<T>.Type];
            }
        }
    }
}

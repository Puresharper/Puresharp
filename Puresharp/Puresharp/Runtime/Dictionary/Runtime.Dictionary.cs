using System;
using System.Linq;
using System.Reflection;

namespace Puresharp
{
    static internal partial class Runtime
    {
        static internal partial class Dictionary
        {
            static public readonly Data.Map<Type, Data.Collection<ConstructorInfo>> Constructors = new Data.Map<Type, Data.Collection<ConstructorInfo>>(_Type => new Data.Collection<ConstructorInfo>(_Type.GetConstructors(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly).OrderBy(_Constructor => _Constructor.MetadataToken).ToArray()), Concurrency.Interlocked);
            static public readonly Data.Map<Type, Data.Collection<FieldInfo>> Fields = new Data.Map<Type, Data.Collection<FieldInfo>>(_Type => new Data.Collection<FieldInfo>(_Type.GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly).OrderBy(_Field => _Field.DeclaringType.MetadataToken).ThenBy(_Field => _Field.MetadataToken).ToArray()), Concurrency.Interlocked);
            static public readonly Data.Map<Type, Data.Collection<PropertyInfo>> Properties = new Data.Map<Type, Data.Collection<PropertyInfo>>(_Type => new Data.Collection<PropertyInfo>(_Type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly).OrderBy(_Property => _Property.DeclaringType.MetadataToken).ThenBy(_Property => _Property.MetadataToken).ToArray()), Concurrency.Interlocked);
            static public readonly Data.Map<Type, Data.Collection<MethodInfo>> Methods = new Data.Map<Type, Data.Collection<MethodInfo>>(_Type => new Data.Collection<MethodInfo>(_Type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly).OrderBy(_Method => _Method.DeclaringType.MetadataToken).ThenBy(_Method => _Method.MetadataToken).ToArray()), Concurrency.Interlocked);
        }
    }

    static internal partial class Runtime<T>
    {
        static internal partial class Dictionary
        {
            static public readonly Data.Collection<ConstructorInfo> Constructors = Runtime.Dictionary.Constructors[Metadata<T>.Type];
            static public readonly Data.Collection<FieldInfo> Fields = Runtime.Dictionary.Fields[Metadata<T>.Type];
            static public readonly Data.Collection<PropertyInfo> Properties = Runtime.Dictionary.Properties[Metadata<T>.Type];
            static public readonly Data.Collection<MethodInfo> Methods = Runtime.Dictionary.Methods[Metadata<T>.Type];
        }
    }
}

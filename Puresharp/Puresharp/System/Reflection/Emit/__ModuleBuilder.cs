using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static internal class __ModuleBuilder
    {
        [DebuggerHidden]
        static public FieldInfo DefineField(this ModuleBuilder module, string name, Type type)
        {
            var _type = module.DefineType(string.Concat(Metadata<Type>.Type.Name, Guid.NewGuid().ToString("N")), TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.Public);
            _type.DefineField(name, type, FieldAttributes.Static | FieldAttributes.Public);
            return _type.CreateType().GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)[0];
        }

        [DebuggerHidden]
        static public FieldInfo DefineField(this ModuleBuilder module, string name, Type type, object value)
        {
            var _field = module.DefineField(name, type);
            _field.SetValue(null, value);
            return _field;
        }

        [DebuggerHidden]
        static public FieldInfo DefineField<T>(this ModuleBuilder module, string name, T value)
        {
            var _field = module.DefineField(name, Metadata<T>.Type);
            _field.SetValue(null, value);
            return _field;
        }

        [DebuggerHidden]
        static public FieldInfo DefineField<T>(this ModuleBuilder module, T value)
        {
            var _field = module.DefineField(Metadata<T>.Type.Name, Metadata<T>.Type);
            _field.SetValue(null, value);
            return _field;
        }

        [DebuggerHidden]
        static public FieldInfo DefineThreadField(this ModuleBuilder module, string name, Type type)
        {
            var _type = module.DefineType(string.Concat(Metadata<Type>.Type.Name, Guid.NewGuid().ToString("N")), TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.Public);
            var _field = _type.DefineField(name, type, FieldAttributes.Static | FieldAttributes.Public);
            _field.SetCustomAttribute(new CustomAttributeBuilder(Metadata.Constructor(() => new ThreadStaticAttribute()), new object[0]));
            return _type.CreateType().GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)[0];
        }
    }
}

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    static internal partial class Runtime
    {
        static public partial class Inventory
        {
            static private ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule();
            static private Data.Map<Type, FieldInfo> m_Types = new Data.Map<Type, FieldInfo>(_Type => typeof(Runtime<>).GetField(Metadata.Field(() => Metadata<object>.Type).Name), Concurrency.Interlocked);
            static private Data.Map<FieldInfo, FieldInfo> m_Fields = new Data.Map<FieldInfo, FieldInfo>(_Field => Runtime.Inventory.Define(_Field), Concurrency.Interlocked);
            static private Data.Map<PropertyInfo, FieldInfo> m_Properties = new Data.Map<PropertyInfo, FieldInfo>(_Property => Runtime.Inventory.Define(_Property), Concurrency.Interlocked);
            static private Data.Map<MethodInfo, FieldInfo> m_Methods = new Data.Map<MethodInfo, FieldInfo>(_Method => Runtime.Inventory.Define(_Method), Concurrency.Interlocked);
            static private Data.Map<ConstructorInfo, FieldInfo> m_Constructors = new Data.Map<ConstructorInfo, FieldInfo>(_Constructor => Runtime.Inventory.Define(_Constructor), Concurrency.Interlocked);
            static private Data.Map<ParameterInfo, FieldInfo> m_Parameters = new Data.Map<ParameterInfo, FieldInfo>(_Parameter => Runtime.Inventory.Define(_Parameter), Concurrency.Interlocked);
            static private Data.Map<MethodBase, FieldInfo> m_Signatures = new Data.Map<MethodBase, FieldInfo>(_Method => Runtime.Inventory.Define(_Method.GetParameters()), Concurrency.Interlocked);

            static private FieldInfo Define<T>(T value)
            {
                var _type = Runtime.Inventory.m_Module.DefineType($"<{ Guid.NewGuid().ToString("N") }>", TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.Serializable);
                _type.DefineField($"<{ Metadata<T>.Type.Name }>", Metadata<T>.Type, FieldAttributes.Public | FieldAttributes.Static);
                var _field = _type.CreateType().GetFields().Single();
                _field.SetValue(null, value);
                return _field;
            }

            static public FieldInfo Type(Type type)
            {
                return Runtime.Inventory.m_Types[type];
            }

            static public FieldInfo Field(FieldInfo field)
            {
                return Runtime.Inventory.m_Fields[field];
            }

            static public FieldInfo Property(PropertyInfo property)
            {
                return Runtime.Inventory.m_Properties[property];
            }

            static public FieldInfo Method(MethodInfo method)
            {
                return Runtime.Inventory.m_Methods[method];
            }

            static public FieldInfo Constructor(ConstructorInfo constructor)
            {
                return Runtime.Inventory.m_Constructors[constructor];
            }

            static public FieldInfo Parameter(ParameterInfo parameter)
            {
                return Runtime.Inventory.m_Parameters[parameter];
            }

            static public FieldInfo Signature(MethodBase method)
            {
                return Runtime.Inventory.m_Signatures[method];
            }
        }
    }
}

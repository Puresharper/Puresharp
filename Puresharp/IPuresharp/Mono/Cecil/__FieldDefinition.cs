using System;
using System.Linq;
using System.Linq.Expressions;
using Mono;
using Mono.Collections.Generic;
using Puresharp;

namespace Mono.Cecil
{
    static internal class __FieldDefinition
    {
        static public CustomAttribute Attribute<T>(this FieldDefinition field)
            where T : Attribute
        {
            var _attribute = new CustomAttribute(field.Module.Import(typeof(T).GetConstructor(Type.EmptyTypes)));
            field.CustomAttributes.Add(_attribute);
            return _attribute;
        }

        static public CustomAttribute Attribute<T>(this FieldDefinition field, Expression<Func<T>> expression)
            where T : Attribute
        {
            var _constructor = (expression.Body as NewExpression).Constructor;
            var _attribute = new CustomAttribute(field.Module.Import(_constructor));
            foreach (var _argument in (expression.Body as NewExpression).Arguments) { _attribute.ConstructorArguments.Add(new CustomAttributeArgument(field.Module.Import(_argument.Type), Expression.Lambda<Func<object>>(Expression.Convert(_argument, Metadata<object>.Type)).Compile()())); }
            field.CustomAttributes.Add(_attribute);
            return _attribute;
        }

        static public FieldReference Relative(this FieldDefinition field)
        {
            var _type = field.DeclaringType.Relative();
            if (_type == field.DeclaringType) { return field; }
            return new FieldReference(field.Name, field.DeclaringType.Module.Import(field.FieldType), _type);
        }

        static public TypeReference Relative(this TypeDefinition type)
        {
            //if (type.DeclaringType == null)
            //{
                if (type.GenericParameters.Count == 0) { return type; }
                return type.MakeGenericType(type.GenericParameters);
            //}
            //else
            //{
            //    var _type = type.DeclaringType.Relative();
            //    _type = _type.Resolve().NestedTypes.Single(_Type => _Type.Name == type.Name);
            //    if (_type.GenericParameters.Count == 0) { return _type; }
            //    return _type.MakeGenericType(_type.GenericParameters);
            //}
        }

        static public MethodReference Relative(this MethodDefinition method)
        {
            var _type = method.DeclaringType.Relative();
            if (_type == method.DeclaringType) { return method; }
            var _method = new MethodReference(method.Name, method.Module.Import(method.ReturnType), _type);
            foreach (var _parameter in method.Parameters) { _method.Parameters.Add(new ParameterDefinition(_parameter.Name, _parameter.Attributes, _parameter.ParameterType)); }
            return _method;
        }
    }
}

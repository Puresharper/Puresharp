using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime;
using System.Runtime.CompilerServices;
using Puresharp;

using PropertyInfo = System.Reflection.PropertyInfo;

namespace Mono.Cecil
{
    static internal class __TypeDefinition
    {
        static public FieldDefinition Field(this TypeDefinition type, string name, FieldAttributes attributes, TypeReference @return)
        {
            var _field = new FieldDefinition(name, attributes, @return);
            type.Fields.Add(_field);
            _field.Attribute<CompilerGeneratedAttribute>();
            _field.Attribute(() => new DebuggerBrowsableAttribute(DebuggerBrowsableState.Never));
            return _field;
        }

        static public FieldDefinition Field(this TypeDefinition type, string name, FieldAttributes attributes, Type @return)
        {
            var _field = new FieldDefinition(name, attributes, type.Module.Import(@return));
            type.Fields.Add(_field);
            _field.Attribute<CompilerGeneratedAttribute>();
            _field.Attribute(() => new DebuggerBrowsableAttribute(DebuggerBrowsableState.Never));
            return _field;
        }

        static public FieldDefinition Field<T>(this TypeDefinition type, FieldAttributes attributes)
        {
            var _field = new FieldDefinition(string.Concat("<", typeof(T).Name, ">"), attributes, type.Module.Import(typeof(T)));
            type.Fields.Add(_field);
            _field.Attribute<CompilerGeneratedAttribute>();
            _field.Attribute(() => new DebuggerBrowsableAttribute(DebuggerBrowsableState.Never));
            return _field;
        }

        static public FieldDefinition Field<T>(this TypeDefinition type, string name, FieldAttributes attributes)
        {
            var _field = new FieldDefinition(name, attributes, type.Module.Import(typeof(T)));
            type.Fields.Add(_field);
            _field.Attribute<CompilerGeneratedAttribute>();
            _field.Attribute(() => new DebuggerBrowsableAttribute(DebuggerBrowsableState.Never));
            return _field;
        }

        static public MethodDefinition Method(this TypeDefinition type, string name, MethodAttributes attributes, TypeReference @return)
        {
            var _method = new MethodDefinition(name, attributes, @return);
            type.Methods.Add(_method);
            //_method.Attribute<CompilerGeneratedAttribute>();
            //_method.Attribute<DebuggerHiddenAttribute>();
            return _method;
        }

        static public MethodDefinition Method(this TypeDefinition type, string name, MethodAttributes attributes, Type @return)
        {
            var _method = new MethodDefinition(name, attributes, type.Module.Import(@return));
            type.Methods.Add(_method);
            //_method.Attribute<CompilerGeneratedAttribute>();
            //_method.Attribute<DebuggerHiddenAttribute>();
            return _method;
        }

        static public MethodDefinition Method(this TypeDefinition type, string name, MethodAttributes attributes)
        {
            var _method = new MethodDefinition(name, attributes, type.Module.TypeSystem.Void);
            type.Methods.Add(_method);
            //_method.Attribute<CompilerGeneratedAttribute>();
            //_method.Attribute<DebuggerHiddenAttribute>();
            return _method;
        }

        static public MethodDefinition Method<T>(this TypeDefinition type, string name, MethodAttributes attributes)
        {
            var _method = new MethodDefinition(name, attributes, type.Module.Import(typeof(T)));
            type.Methods.Add(_method);
            //_method.Attribute<CompilerGeneratedAttribute>();
            //_method.Attribute<DebuggerHiddenAttribute>();
            return _method;
        }

        static public MethodDefinition Method<T>(this TypeDefinition type, MethodAttributes attributes)
        {
            var _method = new MethodDefinition(string.Concat("<", typeof(T).Name, ">"), attributes, type.Module.Import(typeof(T)));
            type.Methods.Add(_method);
            //_method.Attribute<CompilerGeneratedAttribute>();
            //_method.Attribute<DebuggerHiddenAttribute>();
            return _method;
        }

        static public MethodDefinition Implements<T>(this TypeDefinition type, Expression<Func<T, object>> method)
        {
            if (method.Body is UnaryExpression)
            {
                if ((method.Body as UnaryExpression).Operand is MemberExpression) { return type.Implements(type.Module.Import((((method.Body as UnaryExpression).Operand as MemberExpression).Member as PropertyInfo).GetGetMethod())); }
                return type.Implements(type.Module.Import(((method.Body as UnaryExpression).Operand as MethodCallExpression).Method.IsGenericMethod ? ((method.Body as UnaryExpression).Operand as MethodCallExpression).Method.GetGenericMethodDefinition() : ((method.Body as UnaryExpression).Operand as MethodCallExpression).Method));
            }
            if (method.Body is MemberExpression) { return type.Implements(type.Module.Import(((method.Body as MemberExpression).Member as PropertyInfo).GetGetMethod())); }
            return type.Implements(type.Module.Import((method.Body as MethodCallExpression).Method.IsGenericMethod ? (method.Body as MethodCallExpression).Method.GetGenericMethodDefinition() : (method.Body as MethodCallExpression).Method));
        }

        static public MethodDefinition Implements<T>(this TypeDefinition type, Expression<Action<T>> method)
        {
            return type.Implements(type.Module.Import((method.Body as MethodCallExpression).Method.IsGenericMethod ? (method.Body as MethodCallExpression).Method.GetGenericMethodDefinition() : (method.Body as MethodCallExpression).Method));
        }

        static public MethodDefinition Implements(this TypeDefinition type, MethodReference method)
        {
            var _method = new MethodDefinition(string.Concat("<", method.DeclaringType.Name, ".", method.Name, ">"), MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.NewSlot, method.ReturnType);
            _method.CallingConvention = method.CallingConvention;
            if (method.GenericParameters.Count > 0)
            {
                foreach (var _parameter in method.GenericParameters)
                {
                    var _type = new GenericParameter(_parameter.Name, _method);
                    _method.GenericParameters.Add(_type);
                    if (method.ReturnType == _parameter) { _method.ReturnType = _type; }
                }
            }
            foreach (var _parameter in method.Parameters)
            {
                if (_parameter.ParameterType.IsGenericParameter)
                {
                    _method.Parameters.Add(new ParameterDefinition(_parameter.Name, _parameter.Attributes, _method.GenericParameters[method.GenericParameters.IndexOf(_parameter.ParameterType as GenericParameter)]));
                    continue;
                }
                _method.Parameters.Add(new ParameterDefinition(_parameter.Name, _parameter.Attributes, _parameter.ParameterType));
            }
            _method.Overrides.Add(method);
            type.Methods.Add(_method);
            return _method;
        }

        static public TypeDefinition Type(this TypeDefinition type, string name, TypeAttributes attributes)
        {
            var _type = new TypeDefinition(null, name, attributes, type.Module.TypeSystem.Object);
            type.NestedTypes.Add(_type);
            _type.Attribute<CompilerGeneratedAttribute>();
            _type.Attribute<SerializableAttribute>();
            return _type;
        }

        static public TypeDefinition Type<T>(this TypeDefinition type, string name, TypeAttributes attributes)
        {
            var _type = new TypeDefinition(null, name, attributes, type.Module.Import(typeof(T)));
            type.NestedTypes.Add(_type);
            _type.Attribute<CompilerGeneratedAttribute>();
            _type.Attribute<SerializableAttribute>();
            return _type;
        }

        static public CustomAttribute Attribute<T>(this TypeDefinition type)
            where T : Attribute
        {
            var _attribute = new CustomAttribute(type.Module.Import(typeof(T).GetConstructor(System.Type.EmptyTypes)));
            type.CustomAttributes.Add(_attribute);
            return _attribute;
        }

        static public CustomAttribute Attribute<T>(this TypeDefinition type, Expression<Func<T>> expression)
            where T : Attribute
        {
            var _constructor = (expression.Body as NewExpression).Constructor;
            var _attribute = new CustomAttribute(type.Module.Import(_constructor));
            foreach (var _argument in (expression.Body as NewExpression).Arguments) { _attribute.ConstructorArguments.Add(new CustomAttributeArgument(type.Module.Import(_argument.Type), Expression.Lambda<Func<object>>(Expression.Convert(_argument, Metadata<object>.Type)).Compile()())); }
            type.CustomAttributes.Add(_attribute);
            return _attribute;
        }

        static public MethodDefinition Constructor(this TypeDefinition type)
        {
            var _method = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, type.Module.TypeSystem.Void);
            type.Methods.Add(_method);
            _method.Attribute<CompilerGeneratedAttribute>();
            //_method.Attribute<DebuggerHiddenAttribute>();
            return _method;
        }

        static public MethodDefinition Initializer(this TypeDefinition type)
        {
            var _method = new MethodDefinition(".cctor", MethodAttributes.Static | MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, type.Module.TypeSystem.Void);
            type.Methods.Add(_method);
            _method.Attribute<CompilerGeneratedAttribute>();
            //_method.Attribute<DebuggerHiddenAttribute>();
            return _method;
        }

        static public GenericInstanceType MakeGenericType(this TypeReference type, IEnumerable<TypeReference> arguments)
        {
            var _type = new GenericInstanceType(type);
            foreach (var _argument in arguments) { _type.GenericArguments.Add(_argument); }
            return _type;
        }

        static public TypeReference Interface<T>(this TypeDefinition type)
        {
            var _type = type.Module.Import(typeof(T));
            type.Interfaces.Add(new InterfaceImplementation(_type));
            return _type;
        }

        static public GenericParameter GenericParameterType(this TypeReference type, string name)
        {
            if (type is GenericInstanceType) { return type.Resolve().GenericParameters.First(_Type => _Type.Name == name); }
            return type.GenericParameters.First(_Type => _Type.Name == name);
        }
    }
}

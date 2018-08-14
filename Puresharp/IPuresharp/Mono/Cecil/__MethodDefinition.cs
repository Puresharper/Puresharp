using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Puresharp;

using FieldInfo = System.Reflection.FieldInfo;
using MethodBase = System.Reflection.MethodBase;

namespace Mono.Cecil
{
    static internal class __MethodDefinition
    {
        static public CustomAttribute Attribute<T>(this MethodDefinition method)
            where T : Attribute
        {
            var _attribute = new CustomAttribute(method.Module.Import(typeof(T).GetConstructor(Type.EmptyTypes)));
            method.CustomAttributes.Add(_attribute);
            return _attribute;
        }

        static public CustomAttribute Attribute<T>(this MethodDefinition method, Expression<Func<T>> expression)
            where T : Attribute
        {
            var _constructor = (expression.Body as NewExpression).Constructor;
            var _attribute = new CustomAttribute(method.Module.Import(_constructor));
            foreach (var _argument in (expression.Body as NewExpression).Arguments) { _attribute.ConstructorArguments.Add(new CustomAttributeArgument(method.Module.Import(_argument.Type), Expression.Lambda<Func<object>>(Expression.Convert(_argument, Metadata<object>.Type)).Compile()())); }
            method.CustomAttributes.Add(_attribute);
            return _attribute;
        }

        static public bool HasCustomAttribute<T>(this MethodDefinition method)
            where T : Attribute
        {
            return method.CustomAttributes.Any(_Attribute => _Attribute.AttributeType.Resolve() == method.Module.Import(Metadata<T>.Type).Resolve());
        }

        static public ParameterDefinition Parameter(this MethodDefinition method, string name)
        {
            return method.Parameters.Single(_Parameter => _Parameter.Name == name);
        }

        static public void Parameter<T>(this MethodDefinition method)
        {
            method.Parameters.Add(new ParameterDefinition(string.Concat("<", typeof(T).Name, ">"), ParameterAttributes.None, method.Module.Import(typeof(T))));
        }

        static public void Parameter<T>(this MethodDefinition method, string name)
        {
            method.Parameters.Add(new ParameterDefinition(name, ParameterAttributes.None, method.Module.Import(typeof(T))));
        }

        static public void Parameter(this MethodDefinition method, string name, TypeReference type)
        {
            method.Parameters.Add(new ParameterDefinition(name, ParameterAttributes.None, type));
        }

        static public void Parameter(this MethodDefinition method, string name, ParameterAttributes attributes, TypeReference type)
        {
            method.Parameters.Add(new ParameterDefinition(name, attributes, type));
        }

        static public MethodDefinition DefineParameter<T>(this MethodDefinition method, string name)
        {
            method.Parameters.Add(new ParameterDefinition(name, ParameterAttributes.None, method.Module.Import(typeof(T))));
            return method;
        }

        static public MethodDefinition DefineParameter(this MethodDefinition method, string name, TypeReference type)
        {
            method.Parameters.Add(new ParameterDefinition(name, ParameterAttributes.None, type));
            return method;
        }

        static public MethodDefinition DefineReferenceParameter<T>(this MethodDefinition method, string name)
        {
            method.Parameters.Add(new ParameterDefinition(name, ParameterAttributes.In | ParameterAttributes.Out, new ByReferenceType(method.Module.Import(typeof(T)))));
            return method;
        }

        static public MethodDefinition DefineGenericParameter(this MethodDefinition method, string name, string type)
        {
            var _type = new GenericParameter(type, method);
            method.GenericParameters.Add(_type);
            method.Parameters.Add(new ParameterDefinition(name, ParameterAttributes.None, _type));
            return method;
        }

        static public MethodDefinition DefineGenericReferenceParameter(this MethodDefinition method, string name, string type)
        {
            var _type = new GenericParameter(type, method);
            method.GenericParameters.Add(_type);
            method.Parameters.Add(new ParameterDefinition(name, ParameterAttributes.In | ParameterAttributes.Out, new ByReferenceType(_type)));
            return method;
        }

        static public ParameterDefinition Add(this MethodDefinition method, ParameterDefinition parameter)
        {
            method.Parameters.Add(parameter);
            return parameter;
        }

        static public GenericInstanceMethod MakeGenericMethod(this MethodDefinition method, IEnumerable<GenericParameter> arguments)
        {
            var _method = new GenericInstanceMethod(method);
            foreach (var _argument in arguments) { _method.GenericArguments.Add(_argument); }
            return _method;
        }

        static public IEnumerable<TypeReference> GenericParameterTypes(this MethodReference method)
        {
            foreach (var _type in method.DeclaringType.GenericParameterTypes()) { yield return _type; }
            foreach (var _type in method.GenericParameters) { yield return _type; }
            if (method is GenericInstanceMethod) { foreach (var _type in (method as GenericInstanceMethod).GenericArguments) { yield return _type; } }
        }

        static public IEnumerable<TypeReference> GenericParameterTypes(this TypeReference type)
        {
            foreach (var _type in type.GenericParameters) { yield return _type; }
            if (type is GenericInstanceType) { foreach (var _type in (type as GenericInstanceType).GenericArguments) { yield return _type; } }
        }

        //static public MethodReference HHH(this MethodDefinition @this, Dictionary<TypeReference, TypeReference> genericity, MethodReference met)
        //{
        //    if (met is GenericInstanceMethod)
        //    {
        //        if (met.DeclaringType is GenericInstanceType)
        //        {
        //            //var mmm = new MethodReference(met.Name, @this.HHH(genericity, met.ReturnType), @this.HHH(genericity, met.DeclaringType))
        //            //{
        //            //    HasThis = met.HasThis,
        //            //    ExplicitThis = met.ExplicitThis,
        //            //    CallingConvention = met.CallingConvention
        //            //};
        //            //foreach (var parameter in met.Parameters)
        //            //{
        //            //    mmm.Parameters.Add(new ParameterDefinition(@this.HHH(genericity, parameter.ParameterType)));
        //            //}

        //            //foreach (var generic_parameter in met.GenericParameters)
        //            //{
        //            //    mmm.GenericParameters.Add(new GenericParameter(generic_parameter.Name, mmm));
        //            //}
        //            ////var hhhh = new GenericInstanceMethod(mmm);
        //            ////hhhh.
        //            //return mmm.MakeGenericMethod((met as GenericInstanceMethod).GenericArguments.Select(_Type => @this.HHH(genericity, _Type)).ToArray());
        //            //return mmm.MakeGenericMethod((met as GenericInstanceMethod).GenericArguments.Select(_Type => @this.HHH(genericity, _Type)).ToArray());


        //            return @this.Module.Import(@this.Module.Import(met.Resolve())
                        
        //            .MakeGenericMethod((met as GenericInstanceMethod).GenericArguments.Select(_Type => @this.HHH(genericity, _Type)).ToArray())
        //            .MakeHostInstanceGeneric((@this.HHH(genericity, met.DeclaringType) as GenericInstanceType).GenericArguments.ToArray()));

        //            ////var ggg = new GenericInstanceMethod(@this.Module.Import(met.Resolve()));
        //            //ggg.DeclaringType = @this.HHH(genericity, met.DeclaringType);
        //            //ggg.Parameters.Clear();
        //            //foreach (var _parameter in met.Parameters) { ggg.Parameters.Add(new ParameterDefinition(_parameter.Name, _parameter.Attributes, /*@this.HHH(genericity, */_parameter.ParameterType/*)*/)); }
        //            //foreach (var _type in (met as GenericInstanceMethod).GenericArguments) { ggg.GenericArguments.Add(@this.HHH(genericity, _type)); }

        //            //var m = @this.Module.Import(@this.Module.Import(met.Resolve()).MakeHostInstanceGeneric((@this.HHH(genericity, met.DeclaringType) as GenericInstanceType).GenericArguments.Select(_Type => @this.HHH(genericity, _Type)).ToArray()).MakeGenericMethod((met as GenericInstanceMethod).GenericArguments.Select(ppp => @this.HHH(genericity, ppp)).Select(_Type => @this.HHH(genericity, _Type)).ToArray()));
        //            //return met;
        //            //return ggg;
        //        }
        //        else
        //        {
        //            var m =  @this.Module.Import(@this.Module.Import(met.Resolve()).MakeGenericMethod((met as GenericInstanceMethod).GenericArguments.Select(ppp => @this.HHH(genericity, ppp)).ToArray()));
        //            return m;
        //        }
        //    }
        //    else
        //    {
        //        if (met.DeclaringType is GenericInstanceType)
        //        {
        //            var m = @this.Module.Import(met.Resolve()).MakeHostInstanceGeneric((@this.HHH(genericity, met.DeclaringType) as GenericInstanceType).GenericArguments.Select(_Type =>
        //            {
        //                var hhhhhhh = met;
        //                return @this.HHH(genericity, _Type);
        //            }).ToArray());
        //            return m;
        //        }
        //        else
        //        {
        //            return met;
        //        }
        //    }
        //}

        //static public TypeReference HHH(this MethodDefinition @this, Dictionary<TypeReference, TypeReference> genericity, TypeReference type)
        //{
        //    if (type.IsByReference/* || type.Name.EndsWith("&")*/) { return new ByReferenceType(@this.HHH(genericity, (type as ByReferenceType).ElementType)); }
        //    if (genericity.TryGetValue(type, out var _type)) { return _type; }
        //    //var _genericity = @this.DeclaringType.GenericParameters.Concat(@this.GenericParameters).ToArray();
        //    //if (type.Name.StartsWith("!!")) { return _genericity[int.Parse(type.Name.Substring(2)) + @this.DeclaringType.DeclaringType.GenericParameters.Count]; }
        //    //if (type.Name.StartsWith("!")) { return _genericity[int.Parse(type.Name.Substring(1))]; }
        //    if (type is GenericInstanceType) { return @this.Module.Import(type.Resolve()).MakeGenericType((type as GenericInstanceType).GenericArguments.Select(_Type => @this.HHH(genericity, _Type))); }
        //    return type;
        //}

        static public MethodReference Repair(this MethodReference that, MethodReference method)
        {
            var _types = that.GenericParameterTypes().Select(_Type => _Type.Name.StartsWith("!!") ? that.GenericParameters[int.Parse(_Type.Name.Substring(2))] : (_Type.Name.StartsWith("!") ? that.DeclaringType.GenericParameters[int.Parse(_Type.Name.Substring(1))] : _Type)).ToArray();
            if (_types.Length > 0)
            {
                if (method.GenericParameters.Count > 0) //TODO test it!
                {
                    var dec = that.GetRealType(method.DeclaringType);
                    if (dec is GenericInstanceType)
                    {
                        var _method = that.Module.Import(method.Resolve()).MakeHostInstanceGeneric((dec as GenericInstanceType).GenericArguments.ToArray());
                        //var _method = new MethodReference(method.Name, that.GetRealType(method.ReturnType), that.GetRealType(method.DeclaringType));
                        //foreach (var _parameter in method.Parameters) { _method.Parameters.Add(new ParameterDefinition(_parameter.Name, _parameter.Attributes, that.GetRealType(_parameter.ParameterType))); }
                        return _method.MakeGenericMethod(method.GenericParameters.Select(k => k.Name.StartsWith("!!") ? that.GenericParameters[int.Parse(k.Name.Substring(2))] : (k.Name.StartsWith("!") ? that.DeclaringType.GenericParameters[int.Parse(k.Name.Substring(1))] : k)).ToArray());
                    }
                    else
                    {
                        return method.MakeGenericMethod(method.GenericParameters.Select(k => k.Name.StartsWith("!!") ? that.GenericParameters[int.Parse(k.Name.Substring(2))] : (k.Name.StartsWith("!") ? that.DeclaringType.GenericParameters[int.Parse(k.Name.Substring(1))] : k)).ToArray());
                    }
                }
                else if (method is GenericInstanceMethod)
                {
                    var dec = that.GetRealType(method.DeclaringType);
                    if (dec is GenericInstanceType)
                    {
                        var _method = that.Module.Import(method.Resolve()).MakeHostInstanceGeneric((dec as GenericInstanceType).GenericArguments.ToArray());
                        //var _method = new MethodReference(method.Name, that.GetRealType(method.ReturnType), that.GetRealType(method.DeclaringType));
                        //foreach (var _parameter in method.Parameters) { _method.Parameters.Add(new ParameterDefinition(_parameter.Name, _parameter.Attributes, that.GetRealType(_parameter.ParameterType))); }
                        return _method.MakeGenericMethod((method as GenericInstanceMethod).GenericArguments.Select(k => k.Name.StartsWith("!!") ? that.GenericParameters[int.Parse(k.Name.Substring(2))] : (k.Name.StartsWith("!") ? that.DeclaringType.GenericParameters[int.Parse(k.Name.Substring(1))] : k)).ToArray());
                    }
                    else
                    {
                        return method.MakeGenericMethod((method as GenericInstanceMethod).GenericArguments.Select(k => k.Name.StartsWith("!!") ? that.GenericParameters[int.Parse(k.Name.Substring(2))] : (k.Name.StartsWith("!") ? that.DeclaringType.GenericParameters[int.Parse(k.Name.Substring(1))] : k)).ToArray());
                    }
                }
                else
                {
                    var dec = that.GetRealType(method.DeclaringType);
                    if (dec is GenericInstanceType)
                    {
                        var _method = that.Module.Import(method.Resolve()).MakeHostInstanceGeneric((dec as GenericInstanceType).GenericArguments.ToArray());
                        //var _method = new MethodReference(method.Name, that.GetRealType(method.ReturnType), that.GetRealType(method.DeclaringType));
                        //foreach (var _parameter in method.Parameters) { _method.Parameters.Add(new ParameterDefinition(_parameter.Name, _parameter.Attributes, that.GetRealType(_parameter.ParameterType))); }
                        return _method;
                    }
                    else
                    {
                        return method;
                    }
                }
            }
            //var met = method.Resolve();
            //if (met.GenericParameters.Count > 0)
            //{
            //    var kk = method.Module.Import(met).MakeGenericMethod(_types.Reverse().Take(met.GenericParameters.Count).Reverse().ToArray());
            //    if (met.DeclaringType.GenericParameters.Count > 0) { kk.DeclaringType = met.DeclaringType.MakeGenericType(_types.Take(met.DeclaringType.GenericParameters.Count)); }
            //    return kk;
            //}
            //if (met.DeclaringType.GenericParameters.Count > 0)
            //{
            //    var kk = method.Module.Import(met);
            //    kk.DeclaringType = met.DeclaringType.MakeGenericType(_types.Take(met.DeclaringType.GenericParameters.Count));
            //    return kk;
            //}
            return method;
        }


        static public MethodReference Repair2(this MethodReference @this, MethodReference method)
        {
            var _types = @this.GenericParameterTypes().Select(_Type => _Type.Name.StartsWith("!!") ? @this.GenericParameters[int.Parse(_Type.Name.Substring(2))] : (_Type.Name.StartsWith("!") ? @this.DeclaringType.GenericParameters[int.Parse(_Type.Name.Substring(1))] : _Type)).ToArray();
            if (_types.Length > 0)
            {
                if (method.GenericParameters.Count > 0)
                {
                    var _method = new GenericInstanceMethod(@this.Module.Import(method.Resolve()));
                    foreach (var k in method.GenericParameters)
                    {
                        _method.GenericArguments.Add(k.Name.StartsWith("!!") ? @this.GenericParameters[int.Parse(k.Name.Substring(2))] : (k.Name.StartsWith("!") ? @this.DeclaringType.GenericParameters[int.Parse(k.Name.Substring(1))] : k));
                    }
                    _method.DeclaringType = @this.GetRealType(method.DeclaringType);
                    return _method;
                }
                else
                {
                    var dec = @this.GetRealType(method.DeclaringType);
                    if (dec is GenericInstanceType)
                    {
                        var _method = @this.Module.Import(method.Resolve()).MakeHostInstanceGeneric((dec as GenericInstanceType).GenericArguments.ToArray());
                        //var _method = new MethodReference(method.Name, that.GetRealType(method.ReturnType), that.GetRealType(method.DeclaringType));
                        //foreach (var _parameter in method.Parameters) { _method.Parameters.Add(new ParameterDefinition(_parameter.Name, _parameter.Attributes, that.GetRealType(_parameter.ParameterType))); }
                        return _method;
                    }
                    else
                    {
                        return method;
                    }
                }
            }
            //var met = method.Resolve();
            //if (met.GenericParameters.Count > 0)
            //{
            //    var kk = method.Module.Import(met).MakeGenericMethod(_types.Reverse().Take(met.GenericParameters.Count).Reverse().ToArray());
            //    if (met.DeclaringType.GenericParameters.Count > 0) { kk.DeclaringType = met.DeclaringType.MakeGenericType(_types.Take(met.DeclaringType.GenericParameters.Count)); }
            //    return kk;
            //}
            //if (met.DeclaringType.GenericParameters.Count > 0)
            //{
            //    var kk = method.Module.Import(met);
            //    kk.DeclaringType = met.DeclaringType.MakeGenericType(_types.Take(met.DeclaringType.GenericParameters.Count));
            //    return kk;
            //}
            return method;
        }

        static public TypeReference GetRealType(this MethodReference method, TypeReference type)
        {
            if (type.Name.StartsWith("!!"))
            {
                if (method.GenericParameters.Count > 0) { return method.GenericParameters[int.Parse(type.Name.Substring(2))]; }
                return method.Resolve().GenericParameters[int.Parse(type.Name.Substring(2))];
            }
            if (type.Name.StartsWith("!"))
            {
                if (method.DeclaringType.GenericParameters.Count > 0) { return method.DeclaringType.GenericParameters[int.Parse(type.Name.Substring(1))]; }
                return method.DeclaringType.Resolve().GenericParameters[int.Parse(type.Name.Substring(1))];
            }
            if (type.IsGenericParameter) { return method.Resolve().GenericParameterTypes().Reverse().First(_Type => _Type.Name == type.Name); }
            if (type.GenericParameters.Count > 0) { return type.MakeGenericType(type.GenericParameters.Select(_Type => method.GetRealType(_Type))); }
            if (type is GenericInstanceType) { return method.Module.Import(type).MakeGenericType((type as GenericInstanceType).GenericArguments.Select(_Type => method.GetRealType(_Type))); }
            return type;
        }

        static public GenericParameter GenericParameterType(this MethodReference method, string name)
        {
            var _type = method.GenericParameters.SingleOrDefault(_Type => _Type.Name == name);
            if (_type == null) { return method.DeclaringType.GenericParameterType(name); }
            return _type;
        }
    }
}

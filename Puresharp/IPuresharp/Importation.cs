using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace IPuresharp
{
    internal class Importation
    {
        private ModuleDefinition m_Module;
        private Dictionary<TypeReference, TypeReference> m_Dictionary;

        public Importation(IGenericParameterProvider source, IGenericParameterProvider destination)
        {
            this.m_Module = source.Module;
            this.m_Dictionary = new Dictionary<TypeReference, TypeReference>();
            var _source = (source is MethodReference ? (source as MethodReference).DeclaringType.GenericParameters.Concat(source.GenericParameters) : source.GenericParameters).ToArray();
            var _destination = (destination is MethodReference ? (destination as MethodReference).DeclaringType.GenericParameters.Concat(destination.GenericParameters) : destination.GenericParameters).ToArray();
            for (var _index = 0; _index < _source.Length; _index++) { this.m_Dictionary.Add(_source[_index], _destination[_index]); }
        }
        
        public TypeReference this[TypeReference type]
        {
            get
            {
                if (type.IsByReference) { return new ByReferenceType(this[(type as ByReferenceType).ElementType]); }
                if (this.m_Dictionary.TryGetValue(type, out var _type)) { return _type; }
                if (type is GenericInstanceType) { return this.m_Module.Import(type.Resolve()).MakeGenericType((type as GenericInstanceType).GenericArguments.Select(_Type => this[_Type])); }
                return type;
            }
        }

        private TypeReference this[TypeReference type, GenericInstanceMethod method]
        {
            get
            {
                if (type.IsByReference) { return new ByReferenceType(this[(type as ByReferenceType).ElementType, method]); }
                if (this.m_Dictionary.TryGetValue(type, out var _type)) { return _type; }
                if (type is GenericInstanceType) { return this.m_Module.Import(type.Resolve()).MakeGenericType((type as GenericInstanceType).GenericArguments.Select(_Type => this[_Type, method])); }
                if (type.Name.StartsWith("!!")) { return method.Resolve().GenericParameters[int.Parse(type.Name.Substring(2))]; }
                return type;
            }
        }

        public MethodReference this[MethodReference method]
        {
            get
            {
                if (method is GenericInstanceMethod)
                {
                    if (method.DeclaringType is GenericInstanceType)
                    {
                        var test = this.m_Module.Import(method.Resolve()).MakeHostInstanceGeneric((this[method.DeclaringType] as GenericInstanceType).GenericArguments.ToArray());
                        var m = test.MakeGenericMethod((method as GenericInstanceMethod).GenericArguments.Select(_Type => this[_Type]).ToArray());
                        //var k = this.m_Module.Import(method.Resolve()).MakeGenericMethod((method as GenericInstanceMethod).GenericArguments.Select(_Type => this[_Type]).ToArray());
                        //var m = k.MakeHostInstanceGeneric((this[method.DeclaringType] as GenericInstanceType).GenericArguments.ToArray());
                        //m.DeclaringType = this[method.DeclaringType, m as GenericInstanceMethod];
                        //m.ReturnType = this[method.ReturnType, m as GenericInstanceMethod];
                        //foreach (var p in method.Parameters) { m.Parameters[p.Index].ParameterType = this[p.ParameterType, m as GenericInstanceMethod]; }
                        return m;
                    }
                    //        var m = new MethodReference(method.Name, this[method.ReturnType], this[method.DeclaringType])
                    //            {
                    //    HasThis = method.HasThis,
                    //    ExplicitThis = method.ExplicitThis,
                    //    CallingConvention = method.CallingConvention
                    //};
                    //        foreach (var p in method.Parameters) { m.Parameters.Add(new ParameterDefinition(p.Name, p.Attributes, this[p.ParameterType])); }
                    var oo = this.m_Module.Import(method.Resolve()).MakeGenericMethod((method as GenericInstanceMethod).GenericArguments.Select(_Type => this[_Type]).ToArray());
                    //oo.ReturnType = this[method.ReturnType, m as GenericInstanceMethod];
                    //foreach (var p in method.Parameters) { oo.Parameters[p.Index].ParameterType = this[p.ParameterType, oo as GenericInstanceMethod]; }
                    return oo;

                    //var m = new MethodReference(method.Name, this[method.ReturnType], this[method.DeclaringType])
                    //{
                    //    HasThis = method.HasThis,
                    //    ExplicitThis = method.ExplicitThis,
                    //    CallingConvention = method.CallingConvention
                    //};
                    //    foreach (var p in method.Parameters) { m.Parameters.Add(new ParameterDefinition(p.Name, p.Attributes, this[p.ParameterType])); }
                    //    foreach (var p in method.Parameters) { m.Parameters.Add(new ParameterDefinition(p.Name, p.Attributes, this[p.ParameterType])); }
                    //return method;
                }
                else
                {
                    if (method.DeclaringType is GenericInstanceType) { return this.m_Module.Import(method.Resolve()).MakeHostInstanceGeneric((this[method.DeclaringType] as GenericInstanceType).GenericArguments.Select(_Type => this[_Type]).ToArray()); }
                    return method;
                }
            }
        }

        public FieldReference this[FieldReference field]
        {
            get { return field.DeclaringType is GenericInstanceType ? new FieldReference(field.Name, this[field.FieldType], this[field.DeclaringType]) : field; }
        }
    }
}

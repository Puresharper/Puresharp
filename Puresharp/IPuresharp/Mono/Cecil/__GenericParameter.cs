using System;
using Mono;

namespace Mono.Cecil
{
    static internal class __GenericParameter
    {
        static public GenericParameter Copy(this GenericParameter parameter, IGenericParameterProvider owner)
        {
            var _parameter = new GenericParameter(parameter.Name, owner);
            _parameter.Attributes = parameter.Attributes;
            foreach (var _type in parameter.Constraints) { _parameter.Constraints.Add(_type); }
            foreach (var _attribute in parameter.CustomAttributes) { _parameter.CustomAttributes.Add(_attribute); }
            _parameter.HasDefaultConstructorConstraint = parameter.HasDefaultConstructorConstraint;
            _parameter.HasNotNullableValueTypeConstraint = parameter.HasNotNullableValueTypeConstraint;
            _parameter.HasReferenceTypeConstraint = parameter.HasReferenceTypeConstraint;
            _parameter.IsContravariant = parameter.IsContravariant;
            _parameter.IsCovariant = parameter.IsCovariant;
            _parameter.IsNonVariant = _parameter.IsNonVariant;
            _parameter.IsValueType = parameter.IsValueType;
            return _parameter;
        }
    }
}

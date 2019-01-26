using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Puresharp;

namespace IPuresharp
{
    static internal class Bypass
    {
        static public bool Match(TypeDefinition type)
        {
            if (type.Name.StartsWith("<")) { return true; }
            if (type.IsInterface) { return true; }
            if (type.IsValueType) { return true; }
            if (type.Name == Program.Module) { return true; }
            if (type.Is<IAsyncStateMachine>()) { return true; }
            if (type.Is<IAdvice>()) { return true; }
            if (type.Is<Aspect>()) { return true; }
            if (type.Is<Pointcut>()) { return true; }
            if (type.Is<Attribute>()) { return true; }
            if (type.Is<Delegate>()) { return true; }
            if (type.Is<MulticastDelegate>()) { return true; }
            if (type.CustomAttributes.Any(_Attribute => _Attribute.AttributeType.Name == "GeneratedCodeAttribute")) { return true; }
            return false;
        }

        static public bool Match(MethodDefinition method)
        {
            if (method.Name.StartsWith("<")) { return true; }
            if (method.Body == null) { return true; }
            if (method.IsConstructor && method.IsStatic) { return true; }
            if (method.Parameters.Any(_Parameter => _Parameter.ParameterType.IsByReference)) { return true; }
            if (method.CustomAttributes.Any(_Attribute => _Attribute.AttributeType.Name == "GeneratedCodeAttribute")) { return true; }
            return false;
        }
    }
}

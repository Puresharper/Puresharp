using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil.Rocks;

namespace Mono.Cecil
{
    static internal class __MethodReference
    {
        static public MethodReference MakeGenericMethod(this MethodReference method, params TypeReference[] arguments)
        {
            //var _method = new GenericInstanceMethod(method);
            //foreach (var _argument in arguments) { _method.GenericArguments.Add(_argument); }
            //return _method;

            if (arguments.Length == 0)
            {
                return method;
            }

            if (method.GenericParameters.Count != arguments.Length)
            {
                throw new ArgumentException ("Invalid number of generic typearguments supplied");
            }

            var genericTypeRef = new GenericInstanceMethod (method);
                        foreach (var arg in arguments)
            {
                genericTypeRef.GenericArguments.Add (arg);
            }

            return genericTypeRef;
        }

        public static MethodReference MakeHostInstanceGeneric(this MethodReference self, params TypeReference[] arguments)
        {
            var reference = new MethodReference(self.Name, self.ReturnType, self.DeclaringType.MakeGenericInstanceType(arguments))
            {
                HasThis = self.HasThis,
                ExplicitThis = self.ExplicitThis,
                CallingConvention = self.CallingConvention
            };

            foreach (var generic_parameter in self.GenericParameters)
            {
                reference.GenericParameters.Add(new GenericParameter(generic_parameter.Name, reference));
            }

            foreach (var parameter in self.Parameters)
            {
                //if (parameter.ParameterType is GenericParameter)
                //{
                //    var gp = parameter.ParameterType as GenericParameter;
                //    if (gp.Owner == self.DeclaringType)
                //    {
                //        var import = new IPuresharp.Importation(gp.Owner, reference.DeclaringType.Resolve());
                //        reference.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, import[parameter.ParameterType]));
                //    }
                //    else
                //    {
                //        reference.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, parameter.ParameterType));
                //    }
                //}
                //else
                //{
                    reference.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, parameter.ParameterType));
                //}
            }

            return reference;
        }

        //static public GenericInstanceMethod MakeGenericMethod(this MethodReference method, IEnumerable<GenericParameter> arguments)
        //{
        //    var _method = new GenericInstanceMethod(method);
        //    foreach (var _argument in arguments) { _method.GenericArguments.Add(_argument); }
        //    return _method;
        //}
    }
}

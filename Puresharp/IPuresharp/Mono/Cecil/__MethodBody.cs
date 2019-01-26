using System;
using System.Linq;
using Mono;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections;
using Mono.Collections.Generic;
using Puresharp;

using ConstructorInfo = System.Reflection.ConstructorInfo;
using MethodInfo = System.Reflection.MethodInfo;
using FieldInfo = System.Reflection.FieldInfo;
using System.Collections.Generic;

namespace Mono.Cecil
{
    static internal class __MethodBody
    {
        static public readonly MethodInfo GetTypeFromHandle = Puresharp.Metadata.Method(() => Type.GetTypeFromHandle(Metadata<RuntimeTypeHandle>.Value));
        static public readonly MethodInfo GetMethodFromHandle = Puresharp.Metadata.Method(() => MethodInfo.GetMethodFromHandle(Metadata<RuntimeMethodHandle>.Value, Metadata<RuntimeTypeHandle>.Value));
        static public readonly MethodInfo GetMethodFromHandle2 = Puresharp.Metadata.Method(() => MethodInfo.GetMethodFromHandle(Metadata<RuntimeMethodHandle>.Value));

        static public int Add(this MethodBody body, Instruction instruction)
        {
            body.Instructions.Add(instruction);
            var _branch = Branch.Query(body);
            if (_branch == null) { return body.Instructions.Count - 1; }
            _branch.Finialize(instruction);
            return body.Instructions.Count - 1;
        }

        static public IDisposable True(this MethodBody body)
        {
            return new Branch(body, OpCodes.Brfalse).Begin();
        }

        static public IDisposable False(this MethodBody body)
        {
            return new Branch(body, OpCodes.Brtrue).Begin();
        }

        static public IDisposable Lock(this MethodBody body, FieldInfo field)
        {
            return new Lock(body, body.Method.Module.Import(field));
        }

        static public IDisposable Lock(this MethodBody body, FieldReference field)
        {
            return new Lock(body, field);
        }

        static public int Emit(this MethodBody body, Instruction instruction)
        {
            return body.Add(instruction);
        }

        static public int Emit(this MethodBody body, OpCode instruction)
        {
            return body.Add(Instruction.Create(instruction));
        }

        static public int Emit(this MethodBody body, OpCode instruction, Instruction label)
        {
            return body.Add(Instruction.Create(instruction, label));
        }

        static public int Emit(this MethodBody body, OpCode instruction, VariableDefinition variable)
        {
            return body.Add(Instruction.Create(instruction, variable));
        }

        static public int Emit(this MethodBody body, OpCode instruction, FieldInfo field)
        {
            return body.Add(Instruction.Create(instruction, body.Method.DeclaringType.Module.Import(field)));
        }

        static public int Emit(this MethodBody body, OpCode instruction, MethodInfo method)
        {
            return body.Add(Instruction.Create(instruction, body.Method.DeclaringType.Module.Import(method)));
        }

        static public int Emit(this MethodBody body, System.Reflection.BindingFlags binding)
        {
            return body.Emit(OpCodes.Ldc_I4, (int)binding);
        }

        static public int Emit(this MethodBody body, OpCode instruction, TypeReference type, IEnumerable<ParameterDefinition> parameters)
        {
            if (instruction == OpCodes.Calli)
            {
                var _signature = new CallSite(type);
                foreach (var _parameter in parameters) { _signature.Parameters.Add(_parameter); }
                _signature.CallingConvention = MethodCallingConvention.Default;
                return body.Add(Instruction.Create(instruction, _signature));
            }
            throw new InvalidOperationException();
        }

        static public int Emit(this MethodBody body, OpCode instruction, TypeReference type)
        {
            return body.Add(Instruction.Create(instruction, type));
        }

        static public int Emit(this MethodBody body, OpCode instruction, Type type)
        {
            return body.Add(Instruction.Create(instruction, body.Method.Module.Import(type)));
        }

        static public int Emit(this MethodBody body, OpCode instruction, MethodReference method)
        {
            return body.Add(Instruction.Create(instruction, method));
        }

        static public int Emit(this MethodBody body, OpCode instruction, FieldReference field)
        {
            return body.Add(Instruction.Create(instruction, field));
        }

        static public int Emit(this MethodBody body, OpCode instruction, ParameterDefinition parameter)
        {
            return body.Add(Instruction.Create(instruction, parameter));
        }

        static public int Emit(this MethodBody body, OpCode instruction, int operand)
        {
            return body.Add(Instruction.Create(instruction, operand));
        }

        static public int Emit(this MethodBody body, OpCode instruction, string operand)
        {
            return body.Add(Instruction.Create(instruction, operand));
        }

        static public int Emit(this MethodBody body, OpCode instruction, ConstructorInfo constructor)
        {
            return body.Add(Instruction.Create(instruction, body.Method.DeclaringType.Module.Import(constructor)));
        }

        static public int Emit(this MethodBody body, Type type)
        {
            return body.Emit(body.Method.DeclaringType.Module.Import(type));
        }

        static public int Emit(this MethodBody body, TypeReference type)
        {
            body.Emit(OpCodes.Ldtoken, type);
            return body.Emit(OpCodes.Call, __MethodBody.GetTypeFromHandle);
        }

        static public int Emit(this MethodBody body, MethodInfo method)
        {
            return body.Emit(body.Method.DeclaringType.Module.Import(method));
        }

        //static public int Emit(this MethodBody body, MethodReference method)
        //{
        //    body.Emit(OpCodes.Ldtoken, method);
        //    //body.Emit(OpCodes.Ldtoken, method.DeclaringType);
        //    if (method.DeclaringType.GenericParameters.Count == 0) { body.Emit(OpCodes.Ldtoken, method.DeclaringType); }
        //    else { body.Emit(OpCodes.Ldtoken, method.DeclaringType.MakeGenericType(method.DeclaringType.GenericParameters)); }
        //    return body.Emit(OpCodes.Call, __MethodBody.GetMethodFromHandle);
        //}

        static public TypeReference Resolve(this MethodDefinition method, TypeReference type)
        {
            if (type.GenericParameters.Count > 0) { return type.MakeGenericType(type.GenericParameters.Select(_Type => method.Resolve(_Type))); }
            if (type is GenericInstanceType)
            {
                var g = method.Module.Import(type.Resolve()).MakeGenericType((type as GenericInstanceType).GenericArguments.Select(_Type => method.Resolve(_Type)));
                return g;
            }
            return type.IsGenericParameter ? method.GenericParameterType(type.Name) : type;
        }
        
        static public MethodReference Resolve(this MethodBody body, MethodReference method)
        {
            var _method = new MethodReference(method.Name, method.Module.Import(typeof(void)))
            {
                HasThis = method.HasThis,
                ExplicitThis = method.ExplicitThis,
                CallingConvention = method.CallingConvention
            };
            if (method.GenericParameters.Count > 0) { foreach (var _type in method.GenericParameters) { _method.GenericParameters.Add(new GenericParameter(_type.Name, _method)); } }
            if (method is GenericInstanceMethod) { foreach (var _type in (method as GenericInstanceMethod).GenericArguments) { _method.GenericParameters.Add(new GenericParameter(_type.Name, _method)); } }
            _method.DeclaringType = body.Method.Resolve(method.DeclaringType);
            _method.ReturnType = body.Method.Resolve(method.ReturnType);
            foreach (var _parameter in method.Parameters) { _method.Parameters.Add(new ParameterDefinition(_parameter.Name, _parameter.Attributes, body.Method.Resolve(_parameter.ParameterType))); }
            return _method;
        }

        static public int Emit(this MethodBody body, MethodReference method)
        {
            ////if (method is GenericInstanceMethod) { return body.Emit(method.MakeGenericMethod((method as GenericInstanceMethod).GenericArguments.Select(_Type => body.Method.GenericParameterType(_Type.Name)).ToArray())); }
            //if (method.GenericParameters.Count > 0)
            //{
            //    //    var _type1 = body.Method.Resolve(method.DeclaringType.MakeGenericType(body.Method.DeclaringType.GenericParameters));

            //    //var _method = new MethodReference(method.Name, method.Module.Import(typeof(void)));
            //    //foreach (var _type in method.GenericParameters) { _method.GenericParameters.Add(new GenericParameter(_type.Name, _method)); }
            //    //_method.ReturnType = method.GenericParameters.Contains(method.ReturnType) ? _method.GenericParameters.First(_Type => _Type.Name == method.ReturnType.Name) : method.ReturnType;
            //    //_method.DeclaringType = method.DeclaringType.MakeGenericType(body.Method.DeclaringType.GenericParameters.Take(method.DeclaringType.GenericParameters.Count).Concat(_method.GenericParameters));
            //    //foreach (var _parameter in method.Parameters) { _method.Parameters.Add(new ParameterDefinition(_parameter.Name, _parameter.Attributes, method.GenericParameters.Contains(_parameter.ParameterType) ? _method.GenericParameters.First(_Type => _Type.Name == _parameter.ParameterType.Name) : (_parameter.ParameterType.IsGenericParameter ? (_method.DeclaringType as GenericInstanceType).GenericArguments.First(_Type => _Type.Name == _parameter.ParameterType.Name) : _parameter.ParameterType))); }
            //    body.Emit(OpCodes.Ldtoken, method);
            //    body.Emit(OpCodes.Ldtoken, method.DeclaringType);
            //    return body.Emit(OpCodes.Call, __MethodBody.GetMethodFromHandle);
            //    //return body.Emit(_method);
            //}
            body.Emit(OpCodes.Ldtoken, method);
            //body.Emit(OpCodes.Ldtoken, body.Resolve(method));
            body.Emit(OpCodes.Ldtoken, body.Method.Resolve(method.DeclaringType));
            return body.Emit(OpCodes.Call, __MethodBody.GetMethodFromHandle);
        }

        //static public int Emit(this MethodBody body, MethodReference method, Collection<GenericParameter> genericity)
        //{
        //    body.Emit(OpCodes.Ldtoken, method.MakeGenericMethod(genericity.ToArray()));
        //    //body.Emit(OpCodes.Ldtoken, method.DeclaringType);
        //    if (method.DeclaringType.GenericParameters.Count == 0) { body.Emit(OpCodes.Ldtoken, method.DeclaringType); }
        //    else { body.Emit(OpCodes.Ldtoken, method.DeclaringType.MakeGenericType(method.DeclaringType.GenericParameters)); }
        //    return body.Emit(OpCodes.Call, __MethodBody.GetMethodFromHandle);
        //}

        static public VariableDefinition Variable(this MethodBody body, int index)
        {
            return body.Variables.Single(_Variable => _Variable.Index == index);
        }

        static public VariableDefinition Variable<T>(this MethodBody body)
        {
            var _variable = new VariableDefinition(body.Method.DeclaringType.Module.Import(Metadata<T>.Type));
            body.Variables.Add(_variable);
            return _variable;
        }

        static public VariableDefinition Variable(this MethodBody body, TypeReference type)
        {
            var _variable = new VariableDefinition(type);
            body.Variables.Add(_variable);
            return _variable;
        }
    }
}

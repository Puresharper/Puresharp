using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Puresharp;

using Assembly = System.Reflection.Assembly;
using MethodBase = System.Reflection.MethodBase;
using MethodInfo = System.Reflection.MethodInfo;
using ParameterInfo = System.Reflection.ParameterInfo;
using BindingFlags = System.Reflection.BindingFlags;
using CallSite = Mono.Cecil.CallSite;

namespace IPuresharp
{
    public class Authentic
    {
        private TypeDefinition m_Type;
        private Dictionary<MethodDefinition, MethodDefinition> m_Dictionary;

        public Authentic(TypeDefinition type)
        {
            this.m_Type = type.Type("<Authentic>", TypeAttributes.NestedPublic | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract);
            foreach (var _parameter in type.GenericParameters) { this.m_Type.GenericParameters.Add(_parameter.Copy(this.m_Type)); }
            this.m_Dictionary = new Dictionary<MethodDefinition, MethodDefinition>();
            foreach (var _method in type.DeclaringType.Methods.ToArray())
            {
                if (Bypass.Match(_method)) { continue; }
                this.m_Dictionary.Add(_method, this.Manage(_method));
            }
        }

        private MethodDefinition Manage(MethodDefinition method)
        {
            var _type = this.m_Type.Type(method.IsConstructor ? "<<Constructor>>" : $"<{ method.Name }>", TypeAttributes.NestedPublic | TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract);
            foreach (var _generic in method.DeclaringType.GenericParameters.Concat(method.GenericParameters)) { _type.GenericParameters.Add(_generic.Copy(_type)); }
            var _importation = new Importation(method, _type);
            var _method = _type.Method(method.IsConstructor ? "<Constructor>" : method.Name, MethodAttributes.Static | MethodAttributes.Private);
            if (!method.IsStatic) { _method.Parameter("this", ParameterAttributes.None, _method.Resolve(method.DeclaringType)); }
            _method.ReturnType = _method.Resolve(method.ReturnType);
            _method.Body.InitLocals = method.Body.InitLocals;
            _method.DebugInformation.Scope = method.DebugInformation.Scope;
            _method.Body.MaxStackSize = method.Body.MaxStackSize;
            _method.Body.LocalVarToken = method.Body.LocalVarToken;
            foreach (var _parameter in method.Parameters) { _method.Parameter(_parameter.Name, _parameter.Attributes, _method.Resolve(_parameter.ParameterType)); }
            foreach (var _variable in method.Body.Variables) { _method.Body.Variable(_method.Resolve(_variable.VariableType)); }
            var _dictionary = new Dictionary<Instruction, Instruction>();
            foreach (var _instruction in method.Body.Instructions) { _method.Body.Instructions.Add(this.Copy(method, _method, _importation, _instruction, _dictionary)); }
            foreach (var _exception in method.Body.ExceptionHandlers)
            {
                _method.Body.ExceptionHandlers.Add(new ExceptionHandler(_exception.HandlerType)
                {
                    CatchType = _exception.CatchType,
                    TryStart = this.Copy(method, _method, _importation, _exception.TryStart, _dictionary),
                    TryEnd = this.Copy(method, _method, _importation, _exception.TryEnd, _dictionary),
                    HandlerType = _exception.HandlerType,
                    HandlerStart = this.Copy(method, _method, _importation, _exception.HandlerStart, _dictionary),
                    HandlerEnd = this.Copy(method, _method, _importation, _exception.HandlerEnd, _dictionary)
                });
            }
            if (method.DebugInformation != null && method.DebugInformation.Scope != null)
            {
                if (method.DebugInformation.Scope.Variables != null)
                {
                    _method.DebugInformation.Scope.Variables.Clear();
                    foreach (var _variable in method.DebugInformation.Scope.Variables)
                    {
                        _method.DebugInformation.Scope.Variables.Add(new VariableDebugInformation(_method.Body.Variables[_variable.Index], _variable.Name));
                    }
                }
            }
            _method.Body.OptimizeMacros();
            return _method;
        }
        
        private Instruction Copy(MethodDefinition source, MethodDefinition destination, Importation importation, Instruction instruction, Dictionary<Instruction, Instruction> dictionary)
        {
            if (instruction == null) { return null; }
            if (dictionary.TryGetValue(instruction, out var _instruction)) { return _instruction; }
            var _operand = instruction.Operand;
            if (_operand == null) { _instruction = Instruction.Create(instruction.OpCode); }
            else if (_operand is ParameterDefinition) { _instruction = Instruction.Create(instruction.OpCode, destination.Parameter((_operand as ParameterDefinition).Name)); }
            else if (_operand is VariableDefinition) { _instruction = Instruction.Create(instruction.OpCode, destination.Body.Variable((_operand as VariableDefinition).Index)); }
            else if (_operand is FieldReference) { _instruction = Instruction.Create(instruction.OpCode, importation[_operand as FieldReference]); }
            else if (_operand is MethodReference) { _instruction = Instruction.Create(instruction.OpCode, importation[_operand as MethodReference]); }
            else if (_operand is TypeReference) { _instruction = Instruction.Create(instruction.OpCode, importation[_operand as TypeReference]); }
            else if (_operand is string) { _instruction = Instruction.Create(instruction.OpCode, _operand as string); }
            else if (_operand is sbyte) { _instruction = Instruction.Create(instruction.OpCode, (sbyte)_operand); }
            else if (_operand is long) { _instruction = Instruction.Create(instruction.OpCode, (long)_operand); }
            else if (_operand is int) { _instruction = Instruction.Create(instruction.OpCode, (int)_operand); }
            else if (_operand is float) { _instruction = Instruction.Create(instruction.OpCode, (float)_operand); }
            else if (_operand is double) { _instruction = Instruction.Create(instruction.OpCode, (double)_operand); }
            else if (_operand is Instruction) { _instruction = Instruction.Create(instruction.OpCode, this.Copy(source, destination, importation, _operand as Instruction, dictionary)); }
            else if (_operand is Instruction[]) { _instruction = Instruction.Create(instruction.OpCode, (_operand as Instruction[]).Select(_Instruction => this.Copy(source, destination, importation, _Instruction, dictionary)).ToArray()); }
            else if (_operand is CallSite) { _instruction = Instruction.Create(instruction.OpCode, _operand as CallSite); }
            else { throw new NotSupportedException(); }
            //var _sequence = instruction.SequencePoint;
            var _sequence = source.DebugInformation.GetSequencePoint(instruction);
            _instruction.Offset = instruction.Offset;
            if (_sequence != null)
            {
                destination.DebugInformation.SequencePoints.Add
                (
                    new SequencePoint(_instruction, _sequence.Document)
                    {
                        StartLine = _sequence.StartLine,
                        StartColumn = _sequence.StartColumn,
                        EndLine = _sequence.EndLine,
                        EndColumn = _sequence.EndColumn
                    }
                );
                //_instruction.SequencePoint = new SequencePoint(_sequence.Document)
                //{
                //    StartLine = _sequence.StartLine,
                //    StartColumn = _sequence.StartColumn,
                //    EndLine = _sequence.EndLine,
                //    EndColumn = _sequence.EndColumn
                //};
            }
            dictionary.Add(instruction, _instruction);
            return _instruction;
        }

        public TypeDefinition Type
        {
            get { return this.m_Type; }
        }

        public MethodDefinition this[MethodDefinition method]
        {
            get { return this.m_Dictionary[method]; }
        }
    }
}

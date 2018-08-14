using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mono.Cecil.Cil
{
    public class Copy
    {
        private MethodDefinition m_Method;
        private Dictionary<GenericParameter, GenericParameter> m_Genericity;
        private Dictionary<ParameterDefinition, ParameterDefinition> m_Signature;
        private Dictionary<VariableDefinition, VariableDefinition> m_Variation;
        private Dictionary<Instruction, Instruction> m_Dictionary;

        public Copy(MethodDefinition method)
        {
            this.m_Method = method;
            this.m_Dictionary = new Dictionary<Instruction, Instruction>();
        }

        public GenericParameter[] Genericity
        {
            set
            {
                if (this.m_Genericity != null) { throw new NotSupportedException(); }
                this.m_Genericity = new Dictionary<GenericParameter, GenericParameter>();
                for (var _index = 0; _index < this.m_Method.GenericParameters.Count; _index++) { this.m_Genericity.Add(this.m_Method.GenericParameters[_index], value[_index]); }
            }
        }

        public ParameterDefinition[] Signature
        {
            set
            {
                if (this.m_Signature != null) { throw new NotSupportedException(); }
                this.m_Signature = new Dictionary<ParameterDefinition, ParameterDefinition>();
                for (var _index = 0; _index < this.m_Method.Parameters.Count; _index++) { this.m_Signature.Add(this.m_Method.Parameters[_index], value[_index]); }
            }
        }

        public VariableDefinition[] Variation
        {
            set
            {
                if (this.m_Variation != null) { throw new NotSupportedException(); }
                this.m_Variation = new Dictionary<VariableDefinition, VariableDefinition>();
                for (var _index = 0; _index < this.m_Method.Body.Variables.Count; _index++) { this.m_Variation.Add(this.m_Method.Body.Variables[_index], value[_index]); }
            }
        }

        public TypeReference this[TypeReference type]
        {
            get
            {
                if (type is GenericParameter)
                {
                    var _type = this.m_Genericity.TryGetValue(type as GenericParameter);
                    if (_type != null) { return _type; }
                    return null;
                }
                return type;
            }
        }

        public FieldReference this[FieldReference field]
        {
            get { return field; }
        }

        public MethodReference this[MethodReference method]
        {
            get { return method; }
        }

        public Instruction this[Instruction instruction, ModuleDefinition module]
        {
            get
            {
                if (instruction == null) { return null; }
                var _instruction = this.m_Dictionary.TryGetValue(instruction);
                if (_instruction == null)
                {
                    var _operand = instruction.Operand;
                    if (_operand == null) { _instruction = Mono.Cecil.Cil.Instruction.Create(instruction.OpCode); }
                    else if (_operand is ParameterDefinition) { _instruction = Mono.Cecil.Cil.Instruction.Create(instruction.OpCode, this.m_Signature[_operand as ParameterDefinition]); }
                    else if (_operand is VariableDefinition) { _instruction = Mono.Cecil.Cil.Instruction.Create(instruction.OpCode, this.m_Variation[_operand as VariableDefinition]); }
                    else if (_operand is FieldReference) { _instruction = Mono.Cecil.Cil.Instruction.Create(instruction.OpCode, module.Import(this[_operand as FieldReference])); }
                    else if (_operand is MethodReference) { _instruction = Mono.Cecil.Cil.Instruction.Create(instruction.OpCode, module.Import(this[_operand as MethodReference])); }
                    else if (_operand is TypeReference) { _instruction = Mono.Cecil.Cil.Instruction.Create(instruction.OpCode, module.Import(this[_operand as TypeReference])); }
                    else if (_operand is string) { _instruction = Mono.Cecil.Cil.Instruction.Create(instruction.OpCode, _operand as string); }
                    else if (_operand is sbyte) { _instruction = Mono.Cecil.Cil.Instruction.Create(instruction.OpCode, (sbyte)_operand); }
                    else if (_operand is long) { _instruction = Mono.Cecil.Cil.Instruction.Create(instruction.OpCode, (long)_operand); }
                    else if (_operand is int) { _instruction = Mono.Cecil.Cil.Instruction.Create(instruction.OpCode, (int)_operand); }
                    else if (_operand is float) { _instruction = Mono.Cecil.Cil.Instruction.Create(instruction.OpCode, (float)_operand); }
                    else if (_operand is double) { _instruction = Mono.Cecil.Cil.Instruction.Create(instruction.OpCode, (double)_operand); }
                    else if (_operand is Instruction) { _instruction = Mono.Cecil.Cil.Instruction.Create(instruction.OpCode, this[_operand as Instruction, module]); }
                    else if (_operand is Instruction[]) { _instruction = Mono.Cecil.Cil.Instruction.Create(instruction.OpCode, (_operand as Instruction[]).Select(_Instruction => this[_Instruction, module]).ToArray()); }
                    else if (_operand is CallSite) { _instruction = Mono.Cecil.Cil.Instruction.Create(instruction.OpCode, _operand as CallSite); }
                    else { throw new NotSupportedException(); }
                    var _sequence = instruction.SequencePoint;
                    _instruction.Offset = instruction.Offset;
                    if (_sequence != null)
                    {
                        _instruction.SequencePoint = new SequencePoint(_sequence.Document)
                        {
                            StartLine = _sequence.StartLine,
                            StartColumn = _sequence.StartColumn,
                            EndLine = _sequence.EndLine,
                            EndColumn = _sequence.EndColumn
                        };
                    }
                    this.m_Dictionary.Add(instruction, _instruction);
                }
                return _instruction;
            }
        }

        public GenericParameter this[GenericParameter key]
        {
            get { return this.m_Genericity[key]; }
        }

        public ParameterDefinition this[ParameterDefinition key]
        {
            get { return this.m_Signature[key]; }
        }

        public VariableDefinition this[VariableDefinition key]
        {
            get { return this.m_Variation[key]; }
        }
    }
}

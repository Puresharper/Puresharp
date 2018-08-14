using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono;
using Mono.Cecil;

namespace Mono.Cecil.Cil
{
    internal partial class Branch
    {
        static private Dictionary<MethodBody, List<Branch>> m_Dictionary = new Dictionary<MethodBody, List<Branch>>();

        static public Branch Query(MethodBody body)
        {
            if (Branch.m_Dictionary.TryGetValue(body, out var _item))
            {
                var _branch = _item.Last();
                _item.Remove(_branch);
                if (_item.Count == 0) { Branch.m_Dictionary.Remove(body); }
                return _branch;
            }
            return null;
        }

        public readonly MethodBody Body;
        public readonly Instruction Instruction;

        public Branch(MethodBody body, OpCode branch)
        {
            this.Body = body;
            this.Instruction = Mono.Cecil.Cil.Instruction.Create(branch, Mono.Cecil.Cil.Instruction.Create(OpCodes.Nop));
        }

        public IDisposable Begin()
        {
            this.Body.Add(this.Instruction);
            return new Branch.Scope(this);
        }

        public void Finialize(Instruction instruction)
        {
            this.Instruction.Operand = instruction;
        }
    }
}

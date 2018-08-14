using System;
using System.Collections;
using System.Collections.Generic;
using Mono;
using Mono.Cecil;

namespace Mono.Cecil.Cil
{
    internal partial class Branch
    {
        private class Scope : IDisposable
        {
            private Branch m_Branch;

            public Scope(Branch branch)
            {
                this.m_Branch = branch;
            }

            public void Dispose()
            {
                List<Branch> _item;
                if (Branch.m_Dictionary.TryGetValue(this.m_Branch.Body, out _item)) { _item.Add(this.m_Branch); }
                else
                {
                    _item = new List<Branch>();
                    _item.Add(this.m_Branch);
                    Branch.m_Dictionary.Add(this.m_Branch.Body, _item);
                }
            }
        }
    }
}

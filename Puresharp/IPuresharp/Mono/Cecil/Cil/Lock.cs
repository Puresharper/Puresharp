using System;
using System.Reflection;
using System.Threading;
using Puresharp;

namespace Mono.Cecil.Cil
{
    internal partial class Lock : IDisposable
    {
        static private MethodInfo m_Enter = Puresharp.Metadata.Method(() => Monitor.Enter(Metadata<object>.Value));
        static private MethodInfo m_Exit = Puresharp.Metadata.Method(() => Monitor.Exit(Metadata<object>.Value));

        private MethodBody m_Body;
        private FieldReference m_Field;

        public Lock(MethodBody body, FieldReference field)
        {
            this.m_Body = body;
            this.m_Field = field;
            if (field.Resolve().IsStatic)
            {
                body.Emit(OpCodes.Ldsfld, field);
                body.Emit(OpCodes.Call, Lock.m_Enter);
            }
            else
            {
                body.Emit(OpCodes.Ldarg_0);
                body.Emit(OpCodes.Ldsfld, field);
                body.Emit(OpCodes.Call, Lock.m_Enter);
            }
        }

        public void Dispose()
        {
            if (this.m_Field.Resolve().IsStatic)
            {
                this.m_Body.Emit(OpCodes.Ldsfld, this.m_Field);
                this.m_Body.Emit(OpCodes.Call, Lock.m_Enter);
            }
            else
            {
                this.m_Body.Emit(OpCodes.Ldarg_0);
                this.m_Body.Emit(OpCodes.Ldsfld, this.m_Field);
                this.m_Body.Emit(OpCodes.Call, Lock.m_Enter);
            }
        }
    }
}

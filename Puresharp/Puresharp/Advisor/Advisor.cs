using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Puresharp
{
    /// <summary>
    /// Advisor used to advise a method.
    /// </summary>
    public partial class Advisor
    {
        static private Func<IAdvice> m_Null = new Func<IAdvice>(() => new Advice());

        /// <summary>
        /// Null advisor that can create advice doing nothing.
        /// </summary>
        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static public Func<IAdvice> Null
        {
            get { return Advisor.m_Null; }
        }

        private Func<IAdvice> m_Create;

        internal Advisor(Func<IAdvice> create)
        {
            this.m_Create = create;
        }

        internal Func<IAdvice> Create
        {
            get { return this.m_Create; }
        }
    }
}
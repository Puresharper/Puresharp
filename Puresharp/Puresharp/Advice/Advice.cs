using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Puresharp
{
    /// <summary>
    /// Advice.
    /// </summary>
    public partial class Advice : IAdvice
    {
        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool Equals(object left, object right)
        {
            return object.Equals(left, right);
        }

        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }

        static private Func<IAdvice> m_Factory = new Func<IAdvice>(() => new Advice());

        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        static public Func<IAdvice> Factory
        {
            get { return Advice.m_Factory; }
        }

        void IAdvice.Instance<T>(T instance)
        {
        }

        void IAdvice.Argument<T>(ref T value)
        {
        }

        void IAdvice.Begin()
        {
        }

        void IAdvice.Await(MethodInfo method, Task task)
        {
        }

        void IAdvice.Await<T>(MethodInfo method, Task<T> task)
        {
        }

        void IAdvice.Continue()
        {
        }

        void IAdvice.Throw(ref Exception exception)
        {
        }

        void IAdvice.Throw<T>(ref Exception exception, ref T value)
        {
        }

        void IAdvice.Return()
        {
        }

        void IAdvice.Return<T>(ref T value)
        {
        }

        void IDisposable.Dispose()
        {
        }
    }
}
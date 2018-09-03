using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Puresharp.Underground
{
    abstract public class Interceptor : IDisposable
    {
        virtual public void Enter(Invocation invocation)
        {
        }

        virtual public void Exit(Execution execution)
        {
        }

        virtual public void Dispose()
        {
        }

        public class Advice : IAdvice
        {
            private MethodBase m_Method;
            private Interceptor m_Interceptor;
            private object m_Instance;
            private object[] m_Arguments;
            private int m_Index;
            private Invocation m_Invocation;

            public Advice(MethodBase method, Interceptor interceptor)
            {
                this.m_Method = method;
                this.m_Interceptor = interceptor;
                this.m_Arguments = new object[method.GetParameters().Length];
            }

            void IAdvice.Instance<T>(T instance)
            {
                this.m_Instance = instance;
            }

            void IAdvice.Argument<T>(ref T value)
            {
                this.m_Arguments[this.m_Index++] = value;
            }

            void IAdvice.Begin()
            {
                this.m_Interceptor.Enter(this.m_Invocation = new Invocation(this.m_Method, this.m_Instance, this.m_Arguments));
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
                this.m_Interceptor.Exit(new Execution(this.m_Invocation, null, exception));
            }

            void IAdvice.Throw<T>(ref Exception exception, ref T value)
            {
                this.m_Interceptor.Exit(new Execution(this.m_Invocation, null, exception));
            }

            void IAdvice.Return()
            {
                this.m_Interceptor.Exit(new Execution(this.m_Invocation, null, null));
            }

            void IAdvice.Return<T>(ref T value)
            {
                this.m_Interceptor.Exit(new Execution(this.m_Invocation, value, null));
            }

            void IDisposable.Dispose()
            {
                this.m_Interceptor.Dispose();
            }
        }
    }
}

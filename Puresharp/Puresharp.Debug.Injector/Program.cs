using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Puresharp.Debug.Injected;

namespace Puresharp.Debug.Injector
{
    public class Advice1 : IAdvice
    {
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

    public class Aspect1 : Aspect
    {
        public override IEnumerable<Advisor> Manage(MethodBase method)
        {
            //yield return Advice.Parameter
            yield return Advice.For(method).Around(() => new Advice1());
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var aspect1 = new Aspect1();
            aspect1.Weave<Pointcut<Service>>();
            aspect1.Weave(Metadata.Method(() => Demo.Add(Metadata<int>.Value, Metadata<int>.Value)));

            var c = Demo.Add(2, 3);

            var a = 28;
            var demo = new Demo<Guid>();
            var result = demo.World<int>(28, "tutu").GetAwaiter().GetResult();
            var r = demo.Method(28);
            Console.ReadKey();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puresharp.Debug.Injected
{
    public class Service : Attribute
    {
    }

    public interface IDemo<T>
    {
        [Service]
        int Method(int a);
    }

    static internal class Hello<T>
    {
        static public string World(T value)
        {
            return value.ToString();
        }
    }

    public struct ppp
    {
        int m;
            double p;
    }

    static public class Demo
    {
        static public int Add(int a, int b)
        {
            return a + b;
        }
    }

    public class Demo<T> : IDemo<T>
    {
        public async Task Hello()
        {
        }

        public async Task<string> World()
        {
            return "28";
        }

        public async Task<string> World<T1>(T1 a, string b)
        {
            await this.Hello();
            var i = await this.World();
            return a + i + Hello<double>.World(5); ;
        }
        public int Method(int a)
        {
            a = 27;
            return a + 3;
        }

        public string Method(ref string a)
        {
            return a;
        }

        public ppp Method(ref ppp a)
        {
            return Method(ref a);
        }
    }
}

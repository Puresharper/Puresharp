using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puresharp
{
    internal class Module<T> : IModule<T>
        where T : class
    {
        private IResolver m_Resolver;

        public Module(IResolver resolver)
        {
            this.m_Resolver = resolver;
        }

        public T Value 
        {
            get { return this.m_Resolver.Resolve<T>(); }
        }

        public void Dispose()
        {
            this.m_Resolver.Dispose();
        }
    }
}

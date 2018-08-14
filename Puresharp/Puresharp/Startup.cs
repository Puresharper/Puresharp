using System;
using System.Collections.Generic;
using System.Reflection;

namespace Puresharp
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public partial class Startup : Attribute
    {
        static private object m_Handle = new object();
        static private HashSet<MethodBase> m_Handled = new HashSet<MethodBase>();

        static private void Run(MethodInfo method)
        {
            lock (Startup.m_Handle)
            {
                if (Startup.m_Handled.Add(method))
                {
                    (Delegate.CreateDelegate(Metadata<Action>.Type, method) as Action)();
                }
            }
        }
    }
}

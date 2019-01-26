using System;
using System.Linq;

namespace Puresharp
{
    abstract public partial class Aspect
    {
        static public class Monitor
        {
            static public Collection<Aspect> Aspectization
            {
                get { return new Collection<Aspect>(Aspect.m_Aspectization.Where(_Aspect => !(_Aspect is Proxy.Manager)).ToArray()); }
            }

            static public Collection<IWeave> Weaving
            {
                get
                {
                    lock (Aspect.Resource)
                    {
                        return new Collection<IWeave>(Aspect.m_Aspectization.Where(_Aspect => !(_Aspect is Proxy.Manager)).SelectMany(_Aspect => _Aspect.m_Weaving).ToArray());
                    }
                }
            }

            static public Collection<Weave.IConnection> Network
            {
                get
                {
                    lock (Aspect.Resource)
                    {
                        return new Collection<Weave.IConnection>(Aspect.m_Aspectization.Where(_Aspect => !(_Aspect is Proxy.Manager)).SelectMany(_Aspect => _Aspect.m_Network).ToArray());
                    }
                }
            }
        }
    }
}
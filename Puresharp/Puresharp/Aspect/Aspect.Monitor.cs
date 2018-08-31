using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    abstract public partial class Aspect
    {
        static public class Monitor
        {
            static public Collection<Aspect> Aspectization
            {
                get { return new Collection<Aspect>(Aspect.m_Aspectization.ToArray()); }
            }

            static public Collection<IWeave> Weaving
            {
                get
                {
                    lock (Aspect.m_Resource)
                    {
                        return new Collection<IWeave>(Aspect.m_Aspectization.SelectMany(_Aspect => _Aspect.m_Weaving).ToArray());
                    }
                }
            }

            static public Collection<Weave.IConnection> Network
            {
                get
                {
                    lock (Aspect.m_Resource)
                    {
                        return new Collection<Weave.IConnection>(Aspect.m_Aspectization.SelectMany(_Aspect => _Aspect.m_Network).ToArray());
                    }
                }
            }
        }
    }
}
using System;
using System.Reflection;

namespace Puresharp
{ 
    abstract public partial class Aspect
    {
        private class Listener : IDisposable
        {
            private Aspect m_Aspect;
            private Pointcut m_Pointcut;
            private IAudition m_Audition;

            public Listener(Aspect aspect, Pointcut pointcut)
            {
                this.m_Aspect = aspect;
                this.m_Pointcut = pointcut;
                var _listener = new Listener<MethodBase>(_Method =>
                {
                    if (this.m_Pointcut.Match(_Method))
                    {
                        aspect.Weave(_Method);
                    }
                });
                this.m_Audition = Metadata.Functions.Accept(_listener);
            }

            public void Dispose()
            {
                this.m_Audition.Dispose();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Puresharp
{
    abstract public partial class Pointcut
    {
        public class Not<T> : Pointcut
            where T : Pointcut, new()
        {
            sealed override public bool Match(MethodBase method)
            {
                return !Singleton<T>.Value.Match(method);
            }
        }
    }
}

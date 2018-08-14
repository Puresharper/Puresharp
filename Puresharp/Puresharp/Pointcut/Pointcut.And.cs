using System;
using System.Collections.Generic;
using System.Reflection;

namespace Puresharp
{
    abstract public partial class Pointcut
    {
        abstract public class And<T1, T2> : Pointcut
            where T1 : Pointcut, new()
            where T2 : Pointcut, new()
        {
            override public bool Match(MethodBase method)
            {
                return Singleton<T1>.Value.Match(method) && Singleton<T2>.Value.Match(method);
            }
        }
    }
}

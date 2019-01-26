using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Puresharp
{
    static internal class __Aspect
    {
        static public IEnumerable<Func<IAdvice>> Manage(this IEnumerable<Aspect> aspectization, MethodBase method)
        {
            return aspectization.SelectMany(_Aspect => _Aspect.Manage(method).Reverse()).Where(_Advisor => _Advisor != null && _Advisor.Create != null && _Advisor.Create != Advisor.Null).Select(_Advisor => _Advisor.Create);
        }
    }
}

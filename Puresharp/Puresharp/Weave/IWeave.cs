using System;
using System.Reflection;

namespace Puresharp
{
    public interface IWeave
    {
        Aspect Aspect { get; }
        MethodBase Method { get; }
    }
}

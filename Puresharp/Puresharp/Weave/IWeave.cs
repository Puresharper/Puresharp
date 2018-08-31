using System;

namespace Puresharp
{
    public interface IWeave
    {
        Aspect Aspect { get; }
        Pointcut Pointcut { get; }
    }
}

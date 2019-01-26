using System;

namespace Puresharp
{
    public interface IModule<out T> : IDisposable
        where T : class
    {
        T Value { get; }
    }
}

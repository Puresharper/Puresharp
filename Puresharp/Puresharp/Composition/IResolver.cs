using System;

namespace Puresharp
{
    public interface IResolver : IDisposable
    {
        T Resolve<T>()
            where T : class;
    }
}

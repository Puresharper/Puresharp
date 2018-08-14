using System;

namespace Puresharp
{
    public interface IContainer : IDisposable
    {
        IModule<T> Module<T>()
            where T : class;
    }
}

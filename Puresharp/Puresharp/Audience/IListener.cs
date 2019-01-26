using System;

namespace Puresharp
{
    public interface IListener<in T>
        where T : class
    {
        void Listen(T item);
    }
}

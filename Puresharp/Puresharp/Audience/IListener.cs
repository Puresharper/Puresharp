using System;

namespace Puresharp
{
    public interface IListener
    {
        void Listen<T>()
            where T : class;
    }

    public interface IListener<in T>
        where T : class
    {
        void Listen(T item);
    }
}

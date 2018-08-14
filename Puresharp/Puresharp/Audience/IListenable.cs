using System;

namespace Puresharp
{
    public interface IListenable
    {
        IAudition Accept(IListener listener);
    }

    public interface IListenable<T>
        where T : class
    {
        IAudition Accept(IListener<T> listener);
    }
}

using System;

namespace Puresharp
{
    public interface IVisitor<T>
        where T : class
    {
        void Visit(Func<T> value);
    }
}

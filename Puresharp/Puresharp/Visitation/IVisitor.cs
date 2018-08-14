using System;

namespace Puresharp
{
    public interface IVisitor
    {
        void Visit<T>()
            where T : class;
    }

    public interface IVisitor<in T>
        where T : class
    {
        void Visit(T item);
    }
}

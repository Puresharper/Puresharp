using System;

namespace Puresharp
{
    public interface IVisitor
    {
        void Visit<T>(Func<T> value);
    }

    public interface IVisitor<in T>
    {
        void Visit(Func<T> value);
    }
}

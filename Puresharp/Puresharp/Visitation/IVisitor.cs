using System;

namespace Puresharp
{
    public interface IVisitor
    {
        void Visit<T>(Func<T> value);
    }

    public interface IVisitor<T>
        where T : class
    {
        void Visit(Func<T> value);
    }
}

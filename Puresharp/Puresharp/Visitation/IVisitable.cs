using System;

namespace Puresharp
{
    public interface IVisitable
    {
        void Accept(IVisitor visitor);
    }

    public interface IVisitable<T>
        where T : class
    {
        void Accept(IVisitor<T> visitor);
    }
}

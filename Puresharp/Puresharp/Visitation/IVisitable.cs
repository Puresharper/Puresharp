using System;

namespace Puresharp
{
    public interface IVisitable<T>
        where T : class
    {
        void Accept(IVisitor<T> visitor);
    }
}

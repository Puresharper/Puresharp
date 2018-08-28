using System;
using System.Linq.Expressions;

namespace Puresharp
{
    public interface IComposition : IVisitable
    {
        ISetup<T> Setup<T>();
        ISetup<T> Setup<T>(Expression<Func<T>> activation, Instantiation instantiation);
    }
}

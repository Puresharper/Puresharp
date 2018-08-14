using System;
using System.Linq.Expressions;

namespace Puresharp
{
    public interface IComposition : IVisitable
    {
        ISetup<T> Setup<T>()
            where T : class;

        ISetup<T> Setup<T>(Expression<Func<T>> activation, Instantiation instantiation)
            where T : class;
    }
}

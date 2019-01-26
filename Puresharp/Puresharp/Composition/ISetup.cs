using System;
using System.Linq.Expressions;

namespace Puresharp
{
    public interface ISetup<T>
    {
        Expression<Func<T>> Activation { get; set; }
        Instantiation Instantiation { get; set; }
    }
}

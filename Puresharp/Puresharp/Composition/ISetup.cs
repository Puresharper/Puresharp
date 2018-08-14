using System;
using System.Linq.Expressions;

namespace Puresharp
{
    public interface ISetup : IVisitable
    {
        LambdaExpression Activation { get; }
        Instantiation Instantiation { get; }
    }

    public interface ISetup<T> : ISetup
    {
        new Expression<Func<T>> Activation { get; set; }
        new Instantiation Instantiation { get; set; }
    }
}

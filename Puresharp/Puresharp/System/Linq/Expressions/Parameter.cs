using System;
using System.Linq;
using System.Linq.Expressions;

namespace Puresharp
{
    static internal class Parameter<T>
    {
        static public readonly ParameterExpression Expression = System.Linq.Expressions.Expression.Parameter(typeof(T));
    }
}

using System;
using System.Linq.Expressions;

namespace Puresharp
{
    public sealed partial class Composition
    {
        public interface IVisitor
        {
            void Visit<T>(ISetup<T> setup)
                where T : class;
        }
    }
}

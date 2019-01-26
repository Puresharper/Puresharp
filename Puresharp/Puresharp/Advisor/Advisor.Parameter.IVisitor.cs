using System;

namespace Puresharp
{
    public partial class Advisor
    {
        public partial class Parameter
        {
            public interface IVisitor
            {
                void Visit<T>(Func<T> value);
            }
        }
    }
}
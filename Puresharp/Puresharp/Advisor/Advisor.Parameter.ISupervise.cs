using System;
using System.Reflection;

namespace Puresharp
{
    public partial class Advisor
    {
        public partial class Parameter
        {
            public interface ISupervise
            {
                Advisor.IGenerator Generator { get; }
            }
        }

        public partial class Parameter<T>
        {
            public interface ISupervise
            {
                Advisor.IGenerator Generator { get; }
            }
        }
    }
}
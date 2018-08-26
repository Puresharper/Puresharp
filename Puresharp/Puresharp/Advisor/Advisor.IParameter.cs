using System;

namespace Puresharp
{
    public partial class Advisor
    {
        public interface IParameter
        {
            Advisor.IGenerator Generator { get; }
        }

        public interface IParameter<T>
            where T : Attribute
        {
            Advisor.IGenerator Generator { get; }
        }
    }
}
using System;

namespace Puresharp
{
    public partial class Advisor
    {
        public interface IAfter
        {
            Advisor.IGenerator Generator { get; }
        }
    }
}

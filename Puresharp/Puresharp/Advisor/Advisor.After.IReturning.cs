using System;

namespace Puresharp
{
    public partial class Advisor
    {
        public partial class After
        {
            public interface IReturning
            {
                Advisor.IAfter After { get; }
            }
        }
    }
}

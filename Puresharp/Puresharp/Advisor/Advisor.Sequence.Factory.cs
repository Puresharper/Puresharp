using System;

namespace Puresharp
{
    public partial class Advisor
    {
        internal partial class Sequence
        {
            public partial class Factory
            {
                private Func<IAdvice>[] Sequence;

                public Factory(Func<IAdvice>[] sequence)
                {
                    this.Sequence = sequence;
                }

                public IAdvice Create()
                {
                    var _sequence = this.Sequence;
                    var _array = new IAdvice[_sequence.Length];
                    for (var _index = 0; _index < _sequence.Length; _index++) { _array[_index] = _sequence[_index](); }
                    return new Advisor.Sequence(_array);
                }
            }
        }
    }
}
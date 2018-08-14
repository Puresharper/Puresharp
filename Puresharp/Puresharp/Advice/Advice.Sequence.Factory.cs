using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Puresharp
{
    public partial class Advice
    {
        internal partial class Sequence
        {
            public partial class Factory
            {
                private Func<IAdvice>[] Sequence;

                public Factory(Func<IAdvice>[] sequence)
                {
                    var _list = new List<Func<IAdvice>>();
                    foreach (var _factory in sequence) { _list.Add(_factory); }
                    this.Sequence = _list.ToArray();
                }

                public IAdvice Create()
                {
                    var _sequence = this.Sequence;
                    var _array = new IAdvice[_sequence.Length];
                    for (var _index = 0; _index < _sequence.Length; _index++) { _array[_index] = _sequence[_index](); }
                    return new Advice.Sequence(_array);
                }
            }
        }
    }
}
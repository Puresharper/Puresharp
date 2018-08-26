using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Puresharp
{
    public partial class Advisor
    {
        internal partial class Sequence : IAdvice
        {
            private IAdvice[] m_Sequence;

            public Sequence(IAdvice[] sequence)
            {
                this.m_Sequence = sequence;
            }

            public void Instance<T>(T instance)
            {
                var _sequence = this.m_Sequence;
                for (var _index = 0; _index < _sequence.Length; _index++) { _sequence[_index].Instance(instance); }
            }

            public void Argument<T>(ref T value)
            {
                var _sequence = this.m_Sequence;
                for (var _index = 0; _index < _sequence.Length; _index++) { _sequence[_index].Argument(ref value); }
            }

            public void Begin()
            {
                var _sequence = this.m_Sequence;
                for (var _index = 0; _index < _sequence.Length; _index++) { _sequence[_index].Begin(); }
            }

            public void Await(MethodInfo method, Task task)
            {
                var _sequence = this.m_Sequence;
                for (var _index = _sequence.Length - 1; _index >= 0; _index--) { _sequence[_index].Await(method, task); }
            }

            public void Await<T>(MethodInfo method, Task<T> task)
            {
                var _sequence = this.m_Sequence;
                for (var _index = _sequence.Length - 1; _index >= 0; _index--) { _sequence[_index].Await(method, task); }
            }

            public void Continue()
            {
                var _sequence = this.m_Sequence;
                for (var _index = 0; _index < _sequence.Length; _index++) { _sequence[_index].Continue(); }
            }

            public void Throw(ref Exception exception)
            {
                var _sequence = this.m_Sequence;
                for (var _index = _sequence.Length - 1; _index >= 0; _index--) { _sequence[_index].Throw(ref exception); }
            }

            public void Throw<T>(ref Exception exception, ref T value)
            {
                var _sequence = this.m_Sequence;
                for (var _index = _sequence.Length - 1; _index >= 0; _index--) { _sequence[_index].Throw(ref exception, ref value); }
            }

            public void Return()
            {
                var _sequence = this.m_Sequence;
                for (var _index = _sequence.Length - 1; _index >= 0; _index--) { _sequence[_index].Return(); }
            }

            public void Return<T>(ref T value)
            {
                var _sequence = this.m_Sequence;
                for (var _index = _sequence.Length - 1; _index >= 0; _index--) { _sequence[_index].Return(ref value); }
            }

            public void Dispose()
            {
                var _sequence = this.m_Sequence;
                for (var _index = _sequence.Length - 1; _index >= 0; _index--) { _sequence[_index].Dispose(); }
            }
        }
    }
}
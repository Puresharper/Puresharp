using System;
using System.Collections.Generic;
using System.Threading;

namespace Puresharp
{
    static internal partial class Data
    {
        public partial class Map<TKey, TValue>
        {
            private IDictionary<TKey, TValue> m_Dictionary;
            private Func<TKey, TValue> m_Lookup;

            public Map(IDictionary<TKey, TValue> dictionary)
            {
                this.m_Dictionary = dictionary;
                this.m_Lookup = new Func<TKey, TValue>(_Key => dictionary[_Key]);
            }

            public Map(Func<TKey, TValue> lookup, IDictionary<TKey, TValue> dictionary)
            {
                this.m_Dictionary = dictionary;
                this.m_Lookup = new Func<TKey, TValue>(_Key =>
                {
                    TValue _value;
                    if (this.m_Dictionary.TryGetValue(_Key, out _value)) { return _value; }
                    _value = lookup(_Key);
                    this.m_Dictionary.Add(_Key, _value);
                    return _value;
                });
            }

            public Map(Func<TKey, TValue> lookup, Concurrency concurrency = Concurrency.None)
            {
                this.m_Dictionary = new Dictionary<TKey, TValue>();
                switch (concurrency)
                {
                    case Concurrency.None:
                        this.m_Lookup = new Func<TKey, TValue>(_Key =>
                        {
                            TValue _value;
                            if (this.m_Dictionary.TryGetValue(_Key, out _value)) { return _value; }
                            _value = lookup(_Key);
                            this.m_Dictionary.Add(_Key, _value);
                            return _value;
                        });
                        break;
                    case Concurrency.Locked:
                        var _handle = new object();
                        this.m_Lookup = new Func<TKey, TValue>(_Key =>
                        {
                            lock (_handle)
                            {
                                TValue _value;
                                if (this.m_Dictionary.TryGetValue(_Key, out _value)) { return _value; }
                                _value = lookup(_Key);
                                this.m_Dictionary.Add(_Key, _value);
                                return _value;
                            }
                        });
                        break;
                    case Concurrency.Interlocked:
                        this.m_Lookup = new Func<TKey, TValue>(_Key =>
                        {
                            while (true)
                            {
                                TValue _value;
                                var _dictionary = this.m_Dictionary;
                                if (_dictionary.TryGetValue(_Key, out _value)) { return _value; }
                                _value = lookup(_Key);
                                var _substitution = new Dictionary<TKey, TValue>(_dictionary);
                                _substitution.Add(_Key, _value);
                                if (object.ReferenceEquals(Interlocked.CompareExchange(ref this.m_Dictionary, _substitution, _dictionary), _dictionary)) { return _value; }
                            }
                        });
                        break;
                    default: throw new NotSupportedException();
                }
            }

            public TValue this[TKey key]
            {
                get { return this.m_Lookup(key); }
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Puresharp
{
    static internal partial class Data
    {
        public sealed partial class Collection<T> : IEnumerable<T>
        {
            static public implicit operator T[] (Data.Collection<T> collection)
            {
                var _list = collection.m_List;
                var _length = _list.Count;
                var _array = new T[_length];
                for (var _index = 0; _index < _length; _index++) { _array[_index] = _list[_index]; }
                return _array;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            static private T[] m_Empty = new T[0];

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private IList<T> m_List;

            public Collection()
                : this(Data.Collection<T>.m_Empty)
            {
            }

            public Collection(IEnumerable<T> enumerable)
                : this(enumerable is IList<T> ? enumerable as IList<T> : new Enumerable<T>(enumerable))
            {
            }

            public Collection(IList<T> list)
            {
                this.m_List = list;
            }

            public Collection(params T[] array)
                : this(array as IList<T>)
            {
            }

            public Collection(Data.Collection<T> collection)
                : this(collection.m_List)
            {
            }

            public T this[int index]
            {
                get { return this.m_List[index]; }
            }

            public int Count
            {
                get { return this.m_List.Count; }
            }

            public int Index(T value)
            {
                return this.m_List.IndexOf(value);
            }

            public IEnumerator<T> Enumerator()
            {
                return this.m_List.GetEnumerator();
            }

            IEnumerator<T> IEnumerable<T>.GetEnumerator()
            {
                return this.m_List.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.m_List.GetEnumerator();
            }
        }

        [DebuggerDisplay("Count = {this.Count, nq}")]
        [DebuggerTypeProxy(typeof(Data.Collection<>.Debugger))]
        public sealed partial class Collection<T>
        {
            private sealed class Debugger
            {
                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                private Data.Collection<T> m_Collection;

                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                private T[] m_View;

                public Debugger(Data.Collection<T> collection)
                {
                    this.m_Collection = collection;
                }

                [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
                public T[] View
                {
                    get
                    {
                        if (this.m_View == null) { this.m_View = this.m_Collection.ToArray(); }
                        return this.m_View;
                    }
                }
            }
        }

        public partial class Collection<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
        {
            static public implicit operator Data.Map<TKey, TValue>(Data.Collection<TKey, TValue> collection)
            {
                return collection == null ? null : collection.m_Map;
            }

            static private IDictionary<TKey, TValue> m_Empty = new Dictionary<TKey, TValue>();

            private IDictionary<TKey, TValue> m_Dictionary;
            private Data.Map<TKey, TValue> m_Map;

            public Collection()
                : this(Data.Collection<TKey, TValue>.m_Empty)
            {
            }

            public Collection(IDictionary<TKey, TValue> dictionary)
            {
                this.m_Dictionary = dictionary;
                this.m_Map = new Data.Map<TKey, TValue>(dictionary);
            }

            public TValue this[TKey key]
            {
                get { return this.m_Dictionary[key]; }
            }

            public bool Contains(TKey key)
            {
                return this.m_Dictionary.ContainsKey(key);
            }

            public int Count
            {
                get { return this.m_Dictionary.Count; }
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> Enumerator()
            {
                return this.m_Dictionary.GetEnumerator();
            }

            IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
            {
                return this.m_Dictionary.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.m_Dictionary.GetEnumerator();
            }
        }
    }
}

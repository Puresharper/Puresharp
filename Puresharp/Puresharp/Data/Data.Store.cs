using System;
using System.Collections.Generic;

namespace Puresharp
{
    static public partial class Data
    {
        public class Store<T> : IStore<T>
            where T : class
        {
            private Dictionary<string, T> m_Dictionary;

            public Store()
            {
                this.m_Dictionary = new Dictionary<string, T>();
            }

            public T this[string name]
            {
                get
                {
                    this.m_Dictionary.TryGetValue(name, out var _value);
                    return _value;
                }
                set { this.m_Dictionary[name] = value; }
            }

            public void Add(string name, T value)
            {
                this.m_Dictionary.Add(name, value);
            }

            public void Remove(string name)
            {
                this.m_Dictionary.Remove(name);
            }
        }
    }
}

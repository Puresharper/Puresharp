using System;
using System.Collections;
using System.Collections.Generic;

namespace Puresharp
{
    internal sealed class Resource<TKey, TValue>
    {
        private object m_Handle = new object();
        private Dictionary<TKey, TValue> m_Dictionary = new Dictionary<TKey, TValue>();
        private Func<TKey, TValue> m_Activate;

        public Resource(Func<TKey, TValue> activate)
        {
            this.m_Activate = activate;
        }

        public TValue this[TKey key]
        {
            get
            {
                TValue _value;
                if (this.m_Dictionary.TryGetValue(key, out _value)) { return _value; }
                this.m_Dictionary.Add(key, _value = this.m_Activate(key));
                return _value;
            }
        }
    }
}

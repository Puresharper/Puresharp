using System;
using System.Collections.Generic;
using System.Reflection;

namespace Puresharp
{
    public partial class Activity : IActivity
    {
#if NET452
        static private string m_Token = Guid.NewGuid().ToString("N");

        static private Dictionary<Type, object> Dictionary
        {
            get { return System.Runtime.Remoting.Messaging.CallContext.LogicalGetData(Activity.m_Token) as Dictionary<Type, object>;  }
            set { System.Runtime.Remoting.Messaging.CallContext.LogicalSetData(Activity.m_Token, value); }
        }
#else
        static private System.Threading.AsyncLocal<Dictionary<Type, object>> m_Storage = new System.Threading.AsyncLocal<Dictionary<Type, object>>();

        static private Dictionary<Type, object> Dictionary
        {
            get { return Activity.m_Storage.Value;  }
            set { Activity.m_Storage.Value = value; }
        }
#endif

        static public Data.IStore<T> Store<T>()
            where T : class
        {
            var _dictionary = Activity.Dictionary;
            if (_dictionary == null)
            {
                _dictionary = new Dictionary<Type, object>();
                var _store = new Data.Store<T>();
                _dictionary.Add(Metadata<T>.Type, _store);
                Activity.Dictionary = _dictionary;
                return _store;
            }
            else
            {
                if (_dictionary.TryGetValue(Metadata<T>.Type, out var _store)) { return _store as Data.IStore<T>; }
                _store = new Data.Store<T>();
                _dictionary.Add(Metadata<T>.Type, _store);
                return _store as Data.IStore<T>;
            }
        }

        private IActivity m_Authentic;

        public Activity(IActivity authentic)
        {
            this.m_Authentic = authentic;
        }

        public IActivity Authentic
        {
            get { return this.m_Authentic; }
        }

        virtual public void Instance<T>(T instance)
        {
            this.m_Authentic.Instance<T>(instance);
        }

        virtual public void Argument<T>(T value)
        {
            this.m_Authentic.Argument<T>(value);
        }

        virtual public void Invoke()
        {
            this.m_Authentic.Invoke();
        }

        virtual public T Invoke<T>()
        {
            return this.m_Authentic.Invoke<T>();
        }

        public void Dispose()
        {
            this.m_Authentic.Dispose();
        }
    }
}

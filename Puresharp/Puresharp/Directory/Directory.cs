using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Puresharp
{
    public partial class Directory<T> : IDirectory<T>
        where T : class
    {
        private object m_Handle;
        private LinkedList<T> m_Archive;
        private List<IListener<T>> m_Audience;

        public Directory()
        {
            this.m_Handle = new object();
            this.m_Archive = new LinkedList<T>();
            this.m_Audience = new List<IListener<T>>();
        }
        
        public void Add(T item)
        {
            lock (this.m_Handle)
            {
                this.m_Archive.AddLast(item);
                foreach (var _listener in this.m_Audience) { _listener.Listen(item); }
            }
        }

        public void Accept(IVisitor<T> visitor)
        {
            lock (this.m_Handle)
            {
                foreach (var _item in this.m_Archive) { visitor.Visit(_item); }
            }
        }

        public IAudition Accept(IListener<T> listener)
        {
            lock (this.m_Handle)
            {
                this.m_Audience.Add(listener);
                foreach (var _item in this.m_Archive) { listener.Listen(_item); }
                return new Audition(this, listener);
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.m_Archive.ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.m_Archive.ToList().GetEnumerator();
        }
    }
}

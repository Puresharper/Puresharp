using System;
using System.Collections.Generic;

namespace Puresharp
{
    public class Reservation : IDisposable
    {
        private LinkedList<IDisposable> m_Reserve = new LinkedList<IDisposable>();

        public void Add(object item)
        {
            if (item is IDisposable)
            {
                this.m_Reserve.AddLast(item as IDisposable);
            }
        }

        public void Dispose()
        {
            foreach (var _item in this.m_Reserve) { _item.Dispose(); }
        }
    }
}

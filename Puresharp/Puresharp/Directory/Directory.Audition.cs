using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Puresharp
{
    public partial class Directory<T>
    {
        private class Audition : IAudition
        {
            private Directory<T> m_Directory;
            private IListener<T> m_Listener;

            public Audition(Directory<T> directory, IListener<T> listener)
            {
                this.m_Directory = directory;
                this.m_Listener = listener;
            }

            public void Dispose()
            {
                this.m_Directory.m_Audience.Remove(this.m_Listener);
            }
        }
    }
}

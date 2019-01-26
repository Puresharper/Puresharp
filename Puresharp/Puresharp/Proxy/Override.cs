using System;
using System.Reflection;

namespace Puresharp
{
    public class Override<T>
        where T : class
    {
        private Func<T, bool> m_Predicate;
        private Func<MethodInfo, Func<IActivity, IActivity>> m_Overrider;

        public Override(Func<T, bool> predicate, Func<MethodInfo, Func<IActivity, IActivity>> overrider)
        {
            this.m_Predicate = predicate;
            this.m_Overrider = overrider;
        }

        public Func<T, bool> Predicate
        {
            get { return this.m_Predicate; }
        }

        public Func<MethodInfo, Func<IActivity, IActivity>> Overrider
        {
            get { return this.m_Overrider; }
        }
    }
}

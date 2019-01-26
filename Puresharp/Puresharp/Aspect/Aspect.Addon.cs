using System;

namespace Puresharp
{
    abstract public partial class Aspect
    {
        /// <summary>
        /// Specifies that the aspect should behave as an addon.
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
        public class Addon : Attribute
        {
        }
    }
}

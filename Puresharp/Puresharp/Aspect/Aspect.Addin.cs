using System;

namespace Puresharp
{
    abstract public partial class Aspect
    {
        /// <summary>
        /// Specifies that the aspect should behave as an addin.
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
        public class Addin : Attribute
        {
        }
    }
}

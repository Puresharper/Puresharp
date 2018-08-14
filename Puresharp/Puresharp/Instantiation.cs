using System;

namespace Puresharp
{
    /// <summary>
    /// Defines the instantiation strategy of an object.
    /// </summary>
    public enum Instantiation
    {
        /// <summary>
        /// Always istantiate a new instance.
        /// </summary>
        Volatile,

        /// <summary>
        /// Instantiate a new instance for each container.
        /// </summary>
        Multiton,

        /// <summary>
        /// Unique instance for whole composition.
        /// </summary>
        Singleton
    }
}

using System;

namespace Puresharp
{
    internal enum Concurrency
    {
        None = 0,
        Locked = 1,
        Interlocked = 2
    }
}

using System;

namespace Puresharp
{
    static public class Singleton<T>
        where T : class, new()
    {
        static public readonly T Value = new T();
    }
}

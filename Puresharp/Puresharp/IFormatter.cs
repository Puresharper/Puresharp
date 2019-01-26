using System;
using System.IO;

namespace Puresharp
{
    public interface IFormatter
    {
        void Serialize<T>(Stream stream, T value);
        T Deserialize<T>(Stream value);
    }
}

using System;

namespace Puresharp
{
    public interface IValidator
    {
        void Validate<T>(T value);
    }
}

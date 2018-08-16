using System;
using System.Reflection;

namespace Puresharp
{
    public interface IValidator
    {
        void Validate<T>(ParameterInfo parameter, T value);
    }
}

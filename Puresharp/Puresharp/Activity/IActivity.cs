using System;

namespace Puresharp
{
    public interface IActivity : IDisposable
    {
        void Instance<T>(T instance);
        void Argument<T>(T value);
        void Invoke();
        T Invoke<T>();
    }
}

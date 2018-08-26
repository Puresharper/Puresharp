using System;

namespace Puresharp
{
    public interface ISupervisor
    {
        void Supervise<T>(T value);
    }
}

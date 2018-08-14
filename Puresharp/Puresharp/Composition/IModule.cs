using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puresharp
{
    public interface IModule<out T> : IDisposable
        where T : class
    {
        T Value { get; }
    }
}

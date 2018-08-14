using System;
using System.Collections;
using System.Collections.Generic;

namespace Puresharp
{
    public interface IDirectory<T> : IEnumerable<T>, IVisitable<T>, IListenable<T>
        where T : class
    {
    }
}

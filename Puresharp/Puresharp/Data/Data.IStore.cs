using System;

namespace Puresharp
{
    static public partial class Data
    {
        public interface IStore<T>
            where T : class
        {
            T this[string name] { get; set; }
            void Add(string name, T value);
            void Remove(string name);
        }
    }
}

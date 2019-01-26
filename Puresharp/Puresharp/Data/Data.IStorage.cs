using System;
using System.Linq;

namespace Puresharp
{
    static public partial class Data
    {
        public interface IStorage
        {
            IQueryable<T> Query<T>()
                where T : class;

            void Add<T>(T item)
                where T : class;

            void Remove<T>(T item)
                where T : class;
        }
    }
}

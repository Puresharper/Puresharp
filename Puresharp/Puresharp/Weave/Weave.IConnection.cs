using System;
using System.Reflection;

namespace Puresharp
{
    public partial class Weave : IWeave
    {
        public interface IConnection
        {
            Aspect Aspect { get; }
            MethodBase Method { get; }
        }
    }
}

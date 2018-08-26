using System;
using System.Reflection;

namespace Puresharp
{
    public partial class Advisor
    {
        public interface IGenerator
        {
            MethodBase Method { get; }
        }
    }
}
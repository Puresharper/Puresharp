//using System;

//namespace Puresharp
//{
//    /// <summary>
//    /// Apply extension on target type as addin.
//    /// </summary>
//    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true, Inherited=true)]
//    public class Addin : Attribute
//    {
//        /// <summary>
//        /// Extension.
//        /// </summary>
//        public readonly Type Extension;

//        /// <summary>
//        /// Type.
//        /// </summary>
//        public readonly Type Type;

//        /// <summary>
//        /// Declare an addin.
//        /// </summary>
//        /// <param name="extension">Extension : must derived from Extension.</param>
//        /// <param name="type">Type.</param>
//        public Addin(Type extension, Type type)
//        {
//            this.Extension = extension;
//            this.Type = type;
//        }
//    }
//}

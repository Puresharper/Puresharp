using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Puresharp
{
    /// <summary>
    /// Advice.
    /// </summary>
    public interface IAdvice : IDisposable
    {
        /// <summary>
        /// Called when advice is bounded to instance method.
        /// </summary>
        /// <typeparam name="T">Type of instance</typeparam>
        /// <param name="value">Instance</param>
        void Instance<T>(T value);

        /// <summary>
        /// Called for each argument passed.
        /// </summary>
        /// <typeparam name="T">Type of argument</typeparam>
        /// <param name="value">Value</param>
        void Argument<T>(ref T value);

        /// <summary>
        /// Called before calling targeted method
        /// </summary>
        void Begin();

        /// <summary>
        /// Called when method start awaiting asynchronous method without return value.
        /// </summary>
        /// <param name="method">Asynchronous method</param>
        /// <param name="task">Task</param>
        void Await(MethodInfo method, Task task);

        /// <summary>
        /// Called when method start awaiting asynchronous method with a return value.
        /// </summary>
        /// <typeparam name="T">Type of return of the asynchronous method</typeparam>
        /// <param name="method">Asynchronous method</param>
        /// <param name="task">Task</param>
        void Await<T>(MethodInfo method, Task<T> task);

        /// <summary>
        /// Called when awaiting asynchronous method end.
        /// </summary>
        void Continue();

        /// <summary>
        /// Called when method execution without return value throw an exception.
        /// </summary>
        /// <param name="exception">Exception</param>
        void Throw(ref Exception exception);

        /// <summary>
        /// Called when method execution with a return value throw an exception.
        /// </summary>
        /// <typeparam name="T">Type of return value</typeparam>
        /// <param name="exception">Exception</param>
        /// <param name="value">Return value</param>
        void Throw<T>(ref Exception exception, ref T value);

        /// <summary>
        /// Called when method execution end without return value.
        /// </summary>
        void Return();

        /// <summary>
        /// Called when method execution with a return value end.
        /// </summary>
        /// <typeparam name="T">Type of return value</typeparam>
        /// <param name="value">Return value</param>
        void Return<T>(ref T value);
    }
}

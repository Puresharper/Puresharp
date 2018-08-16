using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Puresharp
{
    /// <summary>
    /// Pointcut.
    /// </summary>
    abstract public partial class Pointcut : IEnumerable<MethodBase>, IVisitable<MethodBase>, IListenable<MethodBase>
    {
        /// <summary>
        /// Indicate if method is in pointcut scope.
        /// </summary>
        /// <param name="method">Method</param>
        /// <returns>True if method is in pointcut scope</returns>
        abstract public bool Match(MethodBase method);

        /// <summary>
        /// Visit methods currently match with pointcut predicate.
        /// </summary>
        /// <param name="visitor">Visitor</param>
        public void Accept(IVisitor<MethodBase> visitor)
        {
            Metadata.Functions.Accept
            (
                new Visitor<MethodBase>(_Method =>
                {
                    if (this.Match(_Method))
                    {
                        visitor.Visit(_Method);
                    }
                })
            );
        }

        /// <summary>
        /// Listen methods that match with this pointcut predicate.
        /// </summary>
        /// <param name="listener">Listener</param>
        /// <returns>Audition</returns>
        public IAudition Accept(IListener<MethodBase> listener)
        {
            return Metadata.Functions.Accept
            (
                new Listener<MethodBase>(_Method =>
                {
                    if (this.Match(_Method))
                    {
                        listener.Listen(_Method);
                    }
                })
            );
        }

        /// <summary>
        /// Get an enumerator to enumerate methods that currently match with this pointcut predicate.
        /// </summary>
        /// <returns>Methods that currently match with this pointcut predicate.</returns>
        public IEnumerator<MethodBase> GetEnumerator()
        {
            return Metadata.Functions.Where(this.Match).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    /// <summary>
    /// Pointcut that target all methods with a specific attribute.
    /// </summary>
    /// <typeparam name="T">Type of attribute used to identify a type of method</typeparam>
    public partial class Pointcut<T> : Pointcut
        where T : Attribute
    {
        /// <summary>
        /// Indicate if method is in pointcut scope.
        /// </summary>
        /// <param name="method">Method</param>
        /// <returns>True if method is in pointcut scope</returns>
        sealed override public bool Match(MethodBase method)
        {
            return Attribute<T>.On(method);
        }
    }

    /// <summary>
    /// Pointcut based on attributes junction.
    /// </summary>
    /// <typeparam name="T1">Attribute 1</typeparam>
    /// <typeparam name="T2">Attribute 2</typeparam>
    public partial class Pointcut<T1, T2> : Pointcut
        where T1 : Attribute
        where T2 : Attribute
    {
        /// <summary>
        /// Indicate if method is in pointcut scope.
        /// </summary>
        /// <param name="method">Method</param>
        /// <returns>True if method is in pointcut scope</returns>
        sealed override public bool Match(MethodBase method)
        {
            return Attribute<T1>.On(method) && Attribute<T2>.On(method);
        }
    }

    /// <summary>
    /// Pointcut based on attributes junction.
    /// </summary>
    /// <typeparam name="T1">Attribute 1</typeparam>
    /// <typeparam name="T2">Attribute 2</typeparam>
    /// <typeparam name="T3">Attribute 3</typeparam>
    public partial class Pointcut<T1, T2, T3> : Pointcut
        where T1 : Attribute
        where T2 : Attribute
        where T3 : Attribute
    {
        /// <summary>
        /// Indicate if method is in pointcut scope.
        /// </summary>
        /// <param name="method">Method</param>
        /// <returns>True if method is in pointcut scope</returns>
        sealed override public bool Match(MethodBase method)
        {
            return Attribute<T1>.On(method) && Attribute<T2>.On(method) && Attribute<T3>.On(method);
        }
    }

    /// <summary>
    /// Pointcut based on attributes junction.
    /// </summary>
    /// <typeparam name="T1">Attribute 1</typeparam>
    /// <typeparam name="T2">Attribute 2</typeparam>
    /// <typeparam name="T3">Attribute 3</typeparam>
    /// <typeparam name="T4">Attribute 4</typeparam>
    public partial class Pointcut<T1, T2, T3, T4> : Pointcut
        where T1 : Attribute
        where T2 : Attribute
        where T3 : Attribute
        where T4 : Attribute
    {
        /// <summary>
        /// Indicate if method is in pointcut scope.
        /// </summary>
        /// <param name="method">Method</param>
        /// <returns>True if method is in pointcut scope</returns>
        sealed override public bool Match(MethodBase method)
        {
            return Attribute<T1>.On(method) && Attribute<T2>.On(method) && Attribute<T3>.On(method) && Attribute<T4>.On(method);
        }
    }

    /// <summary>
    /// Pointcut based on attributes junction.
    /// </summary>
    /// <typeparam name="T1">Attribute 1</typeparam>
    /// <typeparam name="T2">Attribute 2</typeparam>
    /// <typeparam name="T3">Attribute 3</typeparam>
    /// <typeparam name="T4">Attribute 4</typeparam>
    /// <typeparam name="T5">Attribute 5</typeparam>
    public partial class Pointcut<T1, T2, T3, T4, T5> : Pointcut
        where T1 : Attribute
        where T2 : Attribute
        where T3 : Attribute
        where T4 : Attribute
        where T5 : Attribute
    {
        /// <summary>
        /// Indicate if method is in pointcut scope.
        /// </summary>
        /// <param name="method">Method</param>
        /// <returns>True if method is in pointcut scope</returns>
        sealed override public bool Match(MethodBase method)
        {
            return Attribute<T1>.On(method) && Attribute<T2>.On(method) && Attribute<T3>.On(method) && Attribute<T4>.On(method) && Attribute<T5>.On(method);
        }
    }

    /// <summary>
    /// Pointcut based on attributes junction.
    /// </summary>
    /// <typeparam name="T1">Attribute 1</typeparam>
    /// <typeparam name="T2">Attribute 2</typeparam>
    /// <typeparam name="T3">Attribute 3</typeparam>
    /// <typeparam name="T4">Attribute 4</typeparam>
    /// <typeparam name="T5">Attribute 5</typeparam>
    /// <typeparam name="T6">Attribute 6</typeparam>
    public partial class Pointcut<T1, T2, T3, T4, T5, T6> : Pointcut
        where T1 : Attribute
        where T2 : Attribute
        where T3 : Attribute
        where T4 : Attribute
        where T5 : Attribute
        where T6 : Attribute
    {
        /// <summary>
        /// Indicate if method is in pointcut scope.
        /// </summary>
        /// <param name="method">Method</param>
        /// <returns>True if method is in pointcut scope</returns>
        sealed override public bool Match(MethodBase method)
        {
            return Attribute<T1>.On(method) && Attribute<T2>.On(method) && Attribute<T3>.On(method) && Attribute<T4>.On(method) && Attribute<T5>.On(method) && Attribute<T6>.On(method);
        }
    }

    /// <summary>
    /// Pointcut based on attributes junction.
    /// </summary>
    /// <typeparam name="T1">Attribute 1</typeparam>
    /// <typeparam name="T2">Attribute 2</typeparam>
    /// <typeparam name="T3">Attribute 3</typeparam>
    /// <typeparam name="T4">Attribute 4</typeparam>
    /// <typeparam name="T5">Attribute 5</typeparam>
    /// <typeparam name="T6">Attribute 6</typeparam>
    /// <typeparam name="T7">Attribute 7</typeparam>
    public partial class Pointcut<T1, T2, T3, T4, T5, T6, T7> : Pointcut
        where T1 : Attribute
        where T2 : Attribute
        where T3 : Attribute
        where T4 : Attribute
        where T5 : Attribute
        where T6 : Attribute
        where T7 : Attribute
    {
        /// <summary>
        /// Indicate if method is in pointcut scope.
        /// </summary>
        /// <param name="method">Method</param>
        /// <returns>True if method is in pointcut scope</returns>
        sealed override public bool Match(MethodBase method)
        {
            return Attribute<T1>.On(method) && Attribute<T2>.On(method) && Attribute<T3>.On(method) && Attribute<T4>.On(method) && Attribute<T5>.On(method) && Attribute<T6>.On(method) && Attribute<T7>.On(method);
        }
    }

    /// <summary>
    /// Pointcut based on attributes junction.
    /// </summary>
    /// <typeparam name="T1">Attribute 1</typeparam>
    /// <typeparam name="T2">Attribute 2</typeparam>
    /// <typeparam name="T3">Attribute 3</typeparam>
    /// <typeparam name="T4">Attribute 4</typeparam>
    /// <typeparam name="T5">Attribute 5</typeparam>
    /// <typeparam name="T6">Attribute 6</typeparam>
    /// <typeparam name="T7">Attribute 7</typeparam>
    /// <typeparam name="T8">Attribute 8</typeparam>
    /// <typeparam name="T9">Attribute 9</typeparam>
    public partial class Pointcut<T1, T2, T3, T4, T5, T6, T7, T8> : Pointcut
        where T1 : Attribute
        where T2 : Attribute
        where T3 : Attribute
        where T4 : Attribute
        where T5 : Attribute
        where T6 : Attribute
        where T7 : Attribute
        where T8 : Attribute
    {
        /// <summary>
        /// Indicate if method is in pointcut scope.
        /// </summary>
        /// <param name="method">Method</param>
        /// <returns>True if method is in pointcut scope</returns>
        sealed override public bool Match(MethodBase method)
        {
            return Attribute<T1>.On(method) && Attribute<T2>.On(method) && Attribute<T3>.On(method) && Attribute<T4>.On(method) && Attribute<T5>.On(method) && Attribute<T6>.On(method) && Attribute<T7>.On(method) && Attribute<T8>.On(method);
        }
    }

    /// <summary>
    /// Pointcut based on attributes junction.
    /// </summary>
    /// <typeparam name="T1">Attribute 1</typeparam>
    /// <typeparam name="T2">Attribute 2</typeparam>
    /// <typeparam name="T3">Attribute 3</typeparam>
    /// <typeparam name="T4">Attribute 4</typeparam>
    /// <typeparam name="T5">Attribute 5</typeparam>
    /// <typeparam name="T6">Attribute 6</typeparam>
    /// <typeparam name="T7">Attribute 7</typeparam>
    /// <typeparam name="T8">Attribute 8</typeparam>
    /// <typeparam name="T9">Attribute 9</typeparam>
    public partial class Pointcut<T1, T2, T3, T4, T5, T6, T7, T8, T9> : Pointcut
        where T1 : Attribute
        where T2 : Attribute
        where T3 : Attribute
        where T4 : Attribute
        where T5 : Attribute
        where T6 : Attribute
        where T7 : Attribute
        where T8 : Attribute
        where T9 : Attribute
    {
        /// <summary>
        /// Indicate if method is in pointcut scope.
        /// </summary>
        /// <param name="method">Method</param>
        /// <returns>True if method is in pointcut scope</returns>
        sealed override public bool Match(MethodBase method)
        {
            return Attribute<T1>.On(method) && Attribute<T2>.On(method) && Attribute<T3>.On(method) && Attribute<T4>.On(method) && Attribute<T5>.On(method) && Attribute<T6>.On(method) && Attribute<T7>.On(method) && Attribute<T8>.On(method) && Attribute<T9>.On(method);
        }
    }
}

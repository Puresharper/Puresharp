using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Puresharp
{
    /// <summary>
    /// Metadata.
    /// </summary>
    static public partial class Metadata
    {
        /// <summary>
        /// Void.
        /// </summary>
        static public readonly Type Void = typeof(void);

        static private Directory<Type> m_Types = new Directory<Type>();
        static private Directory<FieldInfo> m_Fields = new Directory<FieldInfo>();
        static private Directory<ConstructorInfo> m_Constructors = new Directory<ConstructorInfo>();
        static private Directory<MethodInfo> m_Methods = new Directory<MethodInfo>();
        static private Directory<MethodBase> m_Functions = new Directory<MethodBase>();
        static private Directory<PropertyInfo> m_Properties = new Directory<PropertyInfo>();

        static private void Broadcast(object value)
        {
            if (value is Type) { Metadata.m_Types.Add(value as Type); }
            else if (value is FieldInfo) { Metadata.m_Fields.Add(value as FieldInfo); }
            else if (value is ConstructorInfo)
            {
                Metadata.m_Functions.Add(value as MethodBase);
                Metadata.m_Constructors.Add(value as ConstructorInfo);
            }
            else if (value is MethodInfo)
            {
                Metadata.m_Functions.Add(value as MethodBase);
                Metadata.m_Methods.Add(value as MethodInfo);
            }
            else if (value is PropertyInfo) { Metadata.m_Properties.Add(value as PropertyInfo); }
        }

        static public IDirectory<MethodBase> Functions
        {
            get { return Metadata.m_Functions; }
        }

        static public IDirectory<MethodInfo> Methods
        {
            get { return Metadata.m_Methods; }
        }
        
        static public IDirectory<ConstructorInfo> Constructors
        {
            get { return Metadata.m_Constructors; }
        }
        
        static public IDirectory<FieldInfo> Fields
        {
            get { return Metadata.m_Fields; }
        }
        
        static public IDirectory<PropertyInfo> Properties
        {
            get { return Metadata.m_Properties; }
        }
        
        static public IDirectory<Type> Types
        {
            get { return Metadata.m_Types; }
        }
        
        /// <summary>
        /// Obtain constructor from linq expression.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Constructor</returns>
        static public ConstructorInfo Constructor<T>(Expression<Func<T>> expression)
        {
            return (expression.Body as NewExpression).Constructor;
        }
        
        /// <summary>
        /// Obtain static field from linq expression.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Field</returns>
        static public FieldInfo Field<T>(Expression<Func<T>> expression)
        {
            return (expression.Body as MemberExpression).Member as FieldInfo;
        }

        /// <summary>
        /// Obtain static property from linq expression.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>PropertyInfo</returns>
        static public PropertyInfo Property<T>(Expression<Func<T>> expression)
        {
            return (expression.Body as MemberExpression).Member as PropertyInfo;
        }
        
        /// <summary>
        /// Obtain static method from linq expression.
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns>Method</returns>
        static public MethodInfo Method(Expression<Action> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }

        /// <summary>
        /// Obtain static method from linq expression.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Method</returns>
        static public MethodInfo Method<T>(Expression<Func<T>> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal.
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns>Boolean</returns>
        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool Equals(object left, object right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified object instances are the same instance.
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns>Boolean</returns>
        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }
    }

    /// <summary>
    /// Metadata.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    static public partial class Metadata<T>
    {
        static private Type m_Type = typeof(T);
        static public T Value;

        /// <summary>
        /// Type.
        /// </summary>
        static public Type Type
        {
            get { return Metadata<T>.m_Type; }
        }

        /// <summary>
        /// Obtain field from linq expression.
        /// </summary>
        /// <typeparam name="TValue">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Field</returns>
        static public FieldInfo Field<TValue>(Expression<Func<T, TValue>> expression)
        {
            return (expression.Body as MemberExpression).Member as FieldInfo;
        }

        /// <summary>
        /// Obtain property from linq expression.
        /// </summary>
        /// <typeparam name="TValue">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Property</returns>
        static public PropertyInfo Property<TValue>(Expression<Func<T, TValue>> expression)
        {
            return (expression.Body as MemberExpression).Member as PropertyInfo;
        }
        
        /// <summary>
        /// Obtain method from linq expression.
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns>Method</returns>
        static public MethodInfo Method(Expression<Action<T>> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }

        /// <summary>
        /// Obtain method from linq expression.
        /// </summary>
        /// <typeparam name="TReturn">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Method</returns>
        static public MethodInfo Method<TReturn>(Expression<Func<T, TReturn>> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }

        /// <summary>
        /// Determines whether the specified object instances are considered equal.
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns>Boolean</returns>
        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool Equals(object left, object right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// Determines whether the specified object instances are the same instance.
        /// </summary>
        /// <param name="left">left</param>
        /// <param name="right">right</param>
        /// <returns>Boolean</returns>
        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }
    }
}

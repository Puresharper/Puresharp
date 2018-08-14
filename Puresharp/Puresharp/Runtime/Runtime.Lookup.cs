using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Puresharp
{
    static internal partial class Runtime
    {
        static internal partial class Lookup
        {
            static public IEnumerable<FieldInfo> Fields(Type type)
            {
                var _type = type;
                while (_type != null)
                {
                    foreach (var _field in _type.GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)) { yield return _field; }
                    _type = _type.BaseType;
                }
            }
            
            static public IEnumerable<PropertyInfo> Properties(Type type)
            {
                if (type.IsInterface)
                {
                    foreach (var _property in type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)) { yield return _property; }
                    foreach (var _type in type.GetInterfaces()) { foreach (var _property in _type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)) { yield return _property; } }
                }
                else
                {
                    var _type = type;
                    while (_type != null)
                    {
                        foreach (var _property in _type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)) { yield return _property; }
                        _type = _type.BaseType;
                    }
                }
            }
            
            static public IEnumerable<MethodInfo> Methods(Type type)
            {
                if (type.IsInterface)
                {
                    foreach (var _method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)) { yield return _method; }
                    foreach (var _type in type.GetInterfaces()) { foreach (var _method in _type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)) { yield return _method; } }
                }
                else
                {
                    var _dictionary = new HashSet<MethodBase>();
                    var _type = type;
                    while (_type != null)
                    {
                        foreach (var _method in _type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly))
                        {
                            if (_method.IsVirtual)
                            {
                                if (_dictionary.Add(_method.GetBaseDefinition())) { yield return _method; }
                                continue;
                            }
                            yield return _method;
                        }
                        _type = _type.BaseType;
                    }
                }
            }
        }
    }
}

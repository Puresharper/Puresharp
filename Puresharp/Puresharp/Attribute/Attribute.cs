using System;
using System.Linq;
using System.Reflection;

namespace Puresharp
{
    static internal class Attribute<T>
        where T : Attribute
    {
        static public bool On(MethodBase method)
        {
            if (method.GetCustomAttributes(Metadata<T>.Type, true).Any()) { return true; }
            if (Attribute<T>.On(method.DeclaringType)) { return true; }
            foreach (var _property in method.DeclaringType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
            {
                if (_property.GetAccessors().Contains(method))
                {
                    if (_property.GetCustomAttributes(Metadata<T>.Type, true).Any()) { return true; }
                    break;
                }
            }
            var _method = method.GetBaseDefinition();
            if (_method != null)
            {
                if (_method.GetCustomAttributes(Metadata<T>.Type, true).Any()) { return true; }
                foreach (var _property in _method.DeclaringType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                {
                    if (_property.GetAccessors().Contains(_method))
                    {
                        if (_property.GetCustomAttributes(Metadata<T>.Type, true).Any()) { return true; }
                        break;
                    }
                }
            }
            if (method.GetParameters().Any(_Parameter => _Parameter.GetCustomAttributes(Metadata<T>.Type, true).Any())) { return true; }
            foreach (var _map in method.DeclaringType.GetInterfaces().Select(_Interface => method.DeclaringType.GetInterfaceMap(_Interface)))
            {
                for (var _index = 0; _index < _map.TargetMethods.Length; _index++)
                {
                    if (_map.TargetMethods[_index] == method)
                    {
                        if (_map.InterfaceMethods[_index].GetParameters().Any(_Parameter => _Parameter.GetCustomAttributes(Metadata<T>.Type, true).Any())) { return true; }
                    }
                }
            }
            _method = null;
            foreach (var _type in method.DeclaringType.GetInterfaces())
            {
                var _map = method.DeclaringType.GetInterfaceMap(_type);
                for (var _index = 0; _index < _map.InterfaceMethods.Length; _index++)
                {
                    if (_map.TargetMethods[_index] == method)
                    {
                        _method = _map.InterfaceMethods[_index];
                        if (_method.GetCustomAttributes(Metadata<T>.Type, true).Any()) { return true; }
                        foreach (var _property in _method.DeclaringType.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly))
                        {
                            if (_property.GetAccessors().Contains(_method))
                            {
                                if (_property.GetCustomAttributes(Metadata<T>.Type, true).Any()) { return true; }
                                break;
                            }
                        }
                        return false;
                    }
                }
            }
            return false;
        }

        static public bool On(MethodBase method, ParameterInfo parameter)
        {
            foreach (var _map in method.DeclaringType.GetInterfaces().Select(_Interface => method.DeclaringType.GetInterfaceMap(_Interface)))
            {
                for (var _index = 0; _index < _map.TargetMethods.Length; _index++)
                {
                    if (_map.TargetMethods[_index] == method)
                    {
                        if (_map.InterfaceMethods[_index].GetParameters()[parameter.Position].GetCustomAttributes(Metadata<T>.Type, true).Any()) { return true; }
                        return false;
                    }
                }
            }
            return parameter.GetCustomAttributes(Metadata<T>.Type, true).Any();
        }

        static public T From(MethodBase method, ParameterInfo parameter)
        {
            foreach (var _map in method.DeclaringType.GetInterfaces().Select(_Interface => method.DeclaringType.GetInterfaceMap(_Interface)))
            {
                for (var _index = 0; _index < _map.TargetMethods.Length; _index++)
                {
                    if (_map.TargetMethods[_index] == method)
                    {
                        var _attributes = _map.InterfaceMethods[_index].GetParameters()[parameter.Position].GetCustomAttributes(Metadata<T>.Type, true);
                        if (_attributes.Length > 0) { return _attributes.Single() as T; }
                        return null;
                    }
                }
            }
            return parameter.GetCustomAttributes(Metadata<T>.Type, true).Single() as T;
        }

        static public bool On(Type type)
        {
            var _type = type;
            while (true)
            {
                if (_type.GetCustomAttributes(Metadata<T>.Type, true).Any()) { return true; }
                _type = _type.BaseType;
                if (_type == null) { break; }
            }
            foreach (var _interface in type.GetInterfaces()) { if (_interface.GetCustomAttributes(Metadata<T>.Type, true).Any()) { return true; } }
            return false;
        }
    }
}

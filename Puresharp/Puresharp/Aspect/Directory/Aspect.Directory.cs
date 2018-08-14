using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Puresharp
{
    abstract public partial class Aspect
    {
        static private partial class Directory
        {
            static private readonly Dictionary<MethodBase, Aspect.Directory.Entry> m_Dictionary = new Dictionary<MethodBase, Entry>();

            static private Aspect.Directory.Entry Obtain(MethodBase method)
            {
                var _method = method;
                if (_method.DeclaringType != _method.ReflectedType)
                {
                    if (_method is MethodInfo) { _method = (_method as MethodInfo).GetBaseDefinition(); }
                    _method = _method.DeclaringType.FindMembers(MemberTypes.Method, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly, (_Method, _Criteria) => _Method is ConstructorInfo || _Method is MethodInfo && (_Method as MethodInfo).GetBaseDefinition() == _method, null).Single() as MethodBase;
                }
                if (Aspect.Directory.m_Dictionary.TryGetValue(method, out var _entry)) { return _entry; }
                _entry = new Aspect.Directory.Entry(_method.DeclaringType, _method, new Aspect.Activity(_method.DeclaringType, _method));
                Aspect.Directory.m_Dictionary.Add(_method, _entry);
                return _entry;
            }

            static public IEnumerable<MethodBase> Index()
            {
                return Aspect.Directory.m_Dictionary.Values.Where(_Entry => _Entry.Count() > 0).Select(_Entry => _Entry.Method).ToArray();
            }

            static public IEnumerable<MethodBase> Index(Aspect aspect)
            {
                return Aspect.Directory.m_Dictionary.Values.Where(_Entry => _Entry.Contains(aspect)).Select(_Entry => _Entry.Method).ToArray();
            }

            static public IEnumerable<Aspect> Index(MethodBase method)
            {
                return Aspect.Directory.Obtain(method).ToArray();
            }

            static public void Add(MethodBase method, Aspect aspect)
            {
                Aspect.Directory.Obtain(method).Add(aspect);
            }

            static public void Remove(MethodBase method)
            {
                var _entry = Aspect.Directory.Obtain(method);
                foreach (var _aspect in _entry.ToArray()) { _entry.Remove(_aspect); }
            }

            static public void Remove(Aspect aspect)
            {
                foreach (var _method in Aspect.Directory.Index(aspect).ToArray())
                {
                    Aspect.Directory.Obtain(_method).Remove(aspect);
                }
            }

            static public void Remove(MethodBase method, Aspect aspect)
            {
                Aspect.Directory.Obtain(method).Remove(aspect);
            }
        }
    }
}
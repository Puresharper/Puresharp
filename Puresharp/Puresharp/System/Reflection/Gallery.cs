using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    static internal class Gallery
    {
        static private object m_Handle = new object();
        static private ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule();
        static private Dictionary<object, FieldInfo> m_Dictionary = new Dictionary<object, FieldInfo>();

        static public FieldInfo Lookup(FieldInfo field)
        {
            FieldInfo _field;
            lock (Gallery.m_Handle)
            {
                if (Gallery.m_Dictionary.TryGetValue(field, out _field)) { return _field; }
                _field = Gallery.m_Module.DefineField(field);
                Gallery.m_Dictionary.Add(field, _field);
            }
            return _field;
        }

        static public FieldInfo Lookup(MethodBase method)
        {
            FieldInfo _field;
            lock (Gallery.m_Handle)
            {
                if (Gallery.m_Dictionary.TryGetValue(method, out _field)) { return _field; }
                _field = Gallery.m_Module.DefineField(method);
                Gallery.m_Dictionary.Add(method, _field);
            }
            return _field;
        }

        static public FieldInfo Lookup(Type type)
        {
            FieldInfo _field;
            lock (Gallery.m_Handle)
            {
                if (Gallery.m_Dictionary.TryGetValue(type, out _field)) { return _field; }
                _field = Gallery.m_Module.DefineField(type);
                Gallery.m_Dictionary.Add(type, _field);
            }
            return _field;
        }
    }
}

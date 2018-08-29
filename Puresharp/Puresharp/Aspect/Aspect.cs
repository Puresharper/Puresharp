using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    /// <summary>
    /// Aspect.
    /// </summary>
    abstract public partial class Aspect : IDisposable
    {
        internal const string Assembly = "Puresharp.<Aspect>, PublicKey=002400000480000094000000060200000024000052534131000400000100010051A76D0BF5695EB709657A23340D31BF2831DBAEF43A4929F442F960875159CCAD93FBC5994577761C35CFFBA0AEF27255D462A61E1D23D45CF06D9C23FABB59CAB1C6FE510C653CD5843CBC911DBABB0E29201DE8C6035CDEDD3ABEEDBC081C5F85E51D84D6CB068DCF8E9682B2AC3FEE59179221C3E1618A8C3275A8EEDECB";
        static private ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule(Aspect.Assembly.Substring(0, Aspect.Assembly.IndexOf(',')), Aspect.Assembly.Substring(Aspect.Assembly.IndexOf('=') + 1));
        static internal ModuleBuilder Module { get { return Aspect.m_Module; } }

        static private readonly Resource m_Resource = new Resource();
        static private List<Aspect> m_Aspectization = new List<Aspect>();
        
        /// <summary>
        /// Administration.
        /// </summary>
        /// <returns>Administration</returns>
        static public IEnumerable<IWeave> Administration
        {
            get
            {
                lock (Aspect.m_Resource)
                {
                    return Aspect.m_Aspectization.SelectMany(_Aspect => _Aspect.m_Directory).ToArray();
                }
            }
        }

        /// <summary>
        /// Enumerate all aspects woven on a method.
        /// </summary>
        /// <param name="method">Method</param>
        /// <returns>Enumerable of aspects woven on method</returns>
        static public IEnumerable<Aspect> From(MethodBase method)
        {
            lock (Aspect.m_Resource)
            {
                return Aspect.Directory.Index(method);
            }
        }

        /// <summary>
        /// Enumerate all aspects woven on a pointcut.
        /// </summary>
        /// <param name="pointcut">Pointcut</param>
        /// <returns>Enumerable of aspects woven on pointcut</returns>
        static public IEnumerable<Aspect> From(Pointcut pointcut)
        {
            lock (Aspect.m_Resource)
            {
                return Aspect.Administration.Where(_Weave => pointcut.Match(_Weave.Method)).Select(_Weave => _Weave.Aspect).ToArray();
            }
        }

        /// <summary>
        /// Equals.
        /// </summary>
        /// <param name="left">Left</param>
        /// <param name="right">Right</param>
        /// <returns>Equals</returns>
        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool Equals(object left, object right)
        {
            return object.Equals(left, right);
        }

        /// <summary>
        /// ReferenceEquals.
        /// </summary>
        /// <param name="left">Left</param>
        /// <param name="right">Right</param>
        /// <returns>Equals</returns>
        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }

        private Dictionary<Pointcut, Aspect.Listener> m_Dictionary = new Dictionary<Pointcut, Listener>();
        private Directory<IWeave> m_Directory;

        /// <summary>
        /// Create an aspect.
        /// </summary>
        protected Aspect()
        {
            this.m_Directory = new Directory<IWeave>();
            lock (Aspect.m_Resource)
            {
                Aspect.m_Aspectization.Add(this);
            }
        }

        /// <summary>
        /// Advise a method.
        /// </summary>
        /// <param name="method">Method to advise</param>
        /// <returns>Advisors dedicated to method to advise</returns>
        abstract public IEnumerable<Advisor> Manage(MethodBase method);

        /// <summary>
        /// Weave an aspect on a specific method.
        /// </summary>
        /// <param name="method">Method</param>
        public void Weave(MethodBase method)
        {
            if (method.IsAbstract) { throw new InvalidOperationException("Aspect cannot be woven to an abstract method."); }
            lock (Aspect.m_Resource)
            {
                Aspect.Directory.Add(method, this);
                this.m_Directory.Add(new Weave(this, method));
            }
        }

        private void Weave(Pointcut pointcut)
        {
            if (this.m_Dictionary.ContainsKey(pointcut)) { return; }
            this.m_Dictionary.Add(pointcut, new Aspect.Listener(this, pointcut));
        }
        
        /// <summary>
        /// Remove aspect from method.
        /// </summary>
        /// <param name="method">Method</param>
        public void Release(MethodBase method)
        {
            lock (Aspect.m_Resource)
            {
                Aspect.Directory.Remove(method, this);
            }
        }
        
        private void Release(Pointcut pointcut)
        {
            if (this.m_Dictionary.TryGetValue(pointcut, out var _listener))
            {
                _listener.Dispose();
                this.m_Dictionary.Remove(pointcut);
                this.m_Directory.Accept
                (
                    new Visitor<IWeave>(_Weave =>
                    {
                        if (pointcut.Match(_Weave.Method) && !this.m_Dictionary.Keys.Any(_Pointcut => _Pointcut.Match(_Weave.Method)))
                        {
                            this.Release(_Weave.Method);
                        }
                    })
                );
            }
        }

        /// <summary>
        /// Weave an aspect on a pointcut.
        /// </summary>
        /// <typeparam name="T">Pointcut</typeparam>
        public void Weave<T>()
            where T : Pointcut, new()
        {
            lock (Aspect.m_Resource)
            {
                this.Release(Singleton<T>.Value);
                this.Weave(Singleton<T>.Value);
            }
        }

        /// <summary>
        /// Remove aspect from pointcut.
        /// </summary>
        /// <typeparam name="T">Pointcut</typeparam>
        public void Release<T>()
            where T : Pointcut, new()
        {
            lock (Aspect.m_Resource)
            {
                this.Release(Singleton<T>.Value);
            }
        }

        /// <summary>
        /// Remove aspect from all methods.
        /// </summary>
        public void Release()
        {
            lock (Aspect.m_Resource)
            {
                this.m_Directory.Accept(new Visitor<IWeave>(_Weave => this.Release(_Weave.Method)));
            }
        }

        /// <summary>
        /// Dispose aspect and remove it from all methods.
        /// </summary>
        public void Dispose()
        {
            lock (Aspect.m_Resource)
            {
                Aspect.Directory.Remove(this);
                Aspect.m_Aspectization.Remove(this);
            }
        }
    }
}
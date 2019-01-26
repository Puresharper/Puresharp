using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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

        static internal Resource Resource = new Resource();
        static private List<Aspect> m_Aspectization = new List<Aspect>();
        
        /// <summary>
        /// Enumerate all aspects woven on a method.
        /// </summary>
        /// <param name="method">Method</param>
        /// <returns>Enumerable of aspects woven on method</returns>
        static public IEnumerable<Aspect> From(MethodBase method)
        {
            lock (Aspect.Resource)
            {
                return Aspect.Directory.Index(method).Where(_Aspect => !(_Aspect is Proxy.Manager));
            }
        }

        ///// <summary>
        ///// Enumerate all aspects woven on a pointcut.
        ///// </summary>
        ///// <param name="pointcut">Pointcut</param>
        ///// <returns>Enumerable of aspects woven on pointcut</returns>
        //static public IEnumerable<Aspect> From(Pointcut pointcut)
        //{
        //    lock (Aspect.m_Resource)
        //    {
        //        return Aspect.Administration.Where(_Weave => pointcut.Match(_Weave.Method)).Select(_Weave => _Weave.Aspect).ToArray();
        //    }
        //}

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
        private Directory<IWeave> m_Weaving;
        private Directory<Weave.IConnection> m_Network;

        /// <summary>
        /// Create an aspect.
        /// </summary>
        protected Aspect()
        {
            this.m_Weaving = new Directory<IWeave>();
            this.m_Network = new Directory<Weave.IConnection>();
            lock (Aspect.Resource)
            {
                Aspect.m_Aspectization.Add(this);
            }
        }

        public Collection<IWeave> Weaving
        {
            get
            {
                lock (Aspect.Resource)
                {
                    return new Collection<IWeave>(this.m_Weaving);
                }
            }
        }

        public Collection<Weave.IConnection> Network
        {
            get
            {
                lock (Aspect.Resource)
                {
                    return new Collection<Weave.IConnection>(this.m_Network);
                }
            }
        }

        /// <summary>
        /// Advise a method.
        /// </summary>
        /// <param name="method">Method to advise</param>
        /// <returns>Advisors dedicated to method to advise</returns>
        abstract public IEnumerable<Advisor> Manage(MethodBase method);

        private void Weave(MethodBase method)
        {
            if (method.IsAbstract) { throw new InvalidOperationException("Aspect cannot be woven to an abstract method."); }
            lock (Aspect.Resource)
            {
                Aspect.Directory.Add(method, this);
                this.m_Network.Add(new Weave.Connection(this, method));
            }
        }

        private void Weave(Pointcut pointcut)
        {
            if (this.m_Dictionary.ContainsKey(pointcut)) { return; }
            lock (Aspect.Resource)
            {
                this.m_Weaving.Add(new Weave(this, pointcut));
                this.m_Dictionary.Add(pointcut, new Aspect.Listener(this, pointcut));
            }
        }
        
        private void Release(MethodBase method)
        {
            lock (Aspect.Resource)
            {
                Aspect.Directory.Remove(method, this);
            }
        }
        
        private void Release(Pointcut pointcut)
        {
            if (this.m_Dictionary.TryGetValue(pointcut, out var _listener))
            {
                _listener.Dispose();
                lock (Aspect.Resource)
                {
                    foreach (var _weaving in this.m_Weaving.Where(_Weaving => _Weaving.Pointcut == pointcut).ToArray()) { this.m_Weaving.Remove(_weaving); }
                    this.m_Dictionary.Remove(pointcut);
                    foreach (var _connection in this.m_Network.Where(_Connection => _Connection.Aspect == this && pointcut.Match(_Connection.Method)).ToArray()) { this.m_Network.Remove(_connection); }
                }
            }
        }

        /// <summary>
        /// Weave an aspect on a pointcut.
        /// </summary>
        /// <typeparam name="T">Pointcut</typeparam>
        public void Weave<T>()
            where T : Pointcut, new()
        {
            lock (Aspect.Resource)
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
            lock (Aspect.Resource)
            {
                this.Release(Singleton<T>.Value);
            }
        }

        /// <summary>
        /// Remove aspect from all methods.
        /// </summary>
        public void Release()
        {
            lock (Aspect.Resource)
            {
                this.m_Network.Accept(new Visitor<Weave.IConnection>(_Connection => this.Release(_Connection.Method)));
            }
        }

        /// <summary>
        /// Dispose aspect and remove it from all methods.
        /// </summary>
        public void Dispose()
        {
            lock (Aspect.Resource)
            {
                Aspect.Directory.Remove(this);
                Aspect.m_Aspectization.Remove(this);
            }
        }
    }
}
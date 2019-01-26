//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Net;
//using System.Reflection;
//using System.Reflection.Emit;
//using System.Threading.Tasks;
//using System.Web;
//using Microsoft.AspNetCore;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;

//#if NET452
//namespace Microsoft.AspNetCore.Hosting
//{
//    static internal class WebHost
//    {
//         static public WebAppConfig CreateDefaultBuilder(string[] arguments)
//        {
//            return new WebAppConfig();
//        }
//    }

//    internal class WebAppConfig
//    {
//        public WebAppBuilder Configure(Action<IApplicationBuilder> configure)
//        {
//            return new WebAppBuilder();
//        }
//    }

//    internal class WebAppBuilder
//    {
//        public WebAppBuilder UseUrls(params string[] urls)
//        {
//            return this;
//        }

//        public WebApp Build()
//        {
//            return new WebApp();
//        }
//    }

//    internal interface IWebHost : IDisposable
//    {
//        void RunAsync();
//    }

//    internal class WebApp : IWebHost
//    {
//        public void RunAsync()
//        {
//        }

//        public void Dispose()
//        {
//        }
//    }
//}
//namespace Microsoft.AspNetCore.Builder
//{
//    internal interface IApplicationBuilder
//    {
//        IApplicationBuilder Use(Func<RequestDelegate, RequestDelegate> middleware);
//        IApplicationBuilder Map(string pathMatch, Action<IApplicationBuilder> configuration);
//        void Run(RequestDelegate handler);
//    }
//}
//namespace Microsoft.AspNetCore.Http
//{
//    internal delegate Task RequestDelegate(HttpContext context);
//}
//#else
//using Microsoft.Extensions.DependencyInjection;
//#endif

//namespace Puresharp
//{
//    public partial class Communication : IDisposable
//    {
//        public enum Verbs
//        {
//            GET,
//            POST,
//            PUT,
//            PATCH,
//            DELETE
//        }

//        public interface IConvention
//        {
//            IMapping this[MethodInfo method] { get; }
//        }

//        public interface IMapping
//        {
//            string Path { get; }
//            Verbs Verb { get; }
//            IFormatter Formatter { get; }
//            IManager Manager { get; }
//        }

//        public interface IManager
//        {
//            int Code(Exception exception);
//        }

//        public interface IDirectory
//        {
//            void Accept(Directory.IVisitor visitor);
//        }

//        public class Directory : IDirectory
//        {
//            public interface IVisitor
//            {
//                void Visit<T>()
//                    where T : class;
//            }

//            private LinkedList<Action<IVisitor>> m_Visitation = new LinkedList<Action<IVisitor>>();

//            public void Add<T>()
//                where T : class
//            {
//                this.m_Visitation.AddLast(new Action<IVisitor>(_Visitor => _Visitor.Visit<T>()));
//            }

//            public void Accept(IVisitor visitor)
//            {
//                foreach (var _action in this.m_Visitation) { _action(visitor); }
//            }
//        }

//        private class Exposition : Directory.IVisitor
//        {
//            private IContainer m_Container;
//            private IConvention m_Convention;
//            private IApplicationBuilder m_Application;

//            public Exposition(IContainer container, IConvention convention, IApplicationBuilder application)
//            {
//                this.m_Container = container;
//                this.m_Convention = convention;
//                this.m_Application = application;
//            }

//            public void Visit<T>() 
//                where T : class
//            {
//                Communication.Map<T>(this.m_Application, this.m_Container, this.m_Convention);
//            }
//        }

//        private IContainer m_Container;
//        private IConvention m_Convention;
//        private IDirectory m_Directory;
//        private IWebHost m_Hosting;

//        public Communication(IContainer container, IDirectory directory, IConvention convention)
//        {
//            this.m_Container = container;
//            this.m_Directory = directory;
//            this.m_Convention = convention;
//            this.m_Hosting = WebHost.CreateDefaultBuilder(new string[0]).Configure(_Application => this.m_Directory.Accept(new Exposition(this.m_Container, this.m_Convention, _Application))).UseUrls().Build();
//            this.m_Hosting.RunAsync();
//        }

//        public void Dispose()
//        {
//            var _hosting = this.m_Hosting;
//            this.m_Hosting = null;
//            if (_hosting != null) { _hosting.Dispose(); }
//        }

//        static private ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule("<Puresharp.Communication>");

//        static private Type Argumentation(MethodInfo method, ParameterInfo[] signature)
//        {
//            var _type = Communication.m_Module.DefineType($"<Puresharp.Communication.Argumentation>.<{method.DeclaringType.Module.MetadataToken}.{method.DeclaringType.MetadataToken}.{method.MetadataToken}>", TypeAttributes.Class | TypeAttributes.Serializable | TypeAttributes.Sealed | TypeAttributes.Public, Metadata<object>.Type);
//            foreach (var _parameter in signature) { _type.DefineField(_parameter.Name, _parameter.ParameterType, FieldAttributes.Public); }
//            return _type.CreateType();
//        }

//        static private void Map<T>(IApplicationBuilder application, IContainer container, Communication.IConvention convention)
//            where T : class
//        {
//            if (convention == null) { throw new ArgumentNullException(nameof(convention), "Comminication mapping cannot be null."); }
//            foreach (var _method in Metadata<T>.Type.GetMethods())
//            {
//                var _mapping = convention[_method];
//                var _signature = _method.GetParameters();
//                var _instance = Expression.Parameter(Metadata<T>.Type);
//                var _formatter = Expression.Parameter(Metadata<IFormatter>.Type);
//                var _return = Expression.Parameter(Metadata<Stream>.Type);
//                var _type = Communication.Argumentation(_method, _signature);
//                if (_mapping.Verb == Verbs.GET)
//                {
//                    if (_method.ReturnType == Metadata.Void) { throw new InvalidOperationException("Verb cannot be 'GET' for a method that has no return."); }
//                    if (_signature.Length == 1)
//                    {
//                        var _parameter = _signature[0];
//                        if (_parameter.ParameterType == Metadata<string>.Type)
//                        {
//                            var _argument = Expression.Parameter(Metadata<string>.Type);
//                            var _action = Expression.Lambda<Action<T, string, IFormatter, Stream>>
//                            (
//                                Expression.Call
//                                (
//                                    _formatter,
//                                    Metadata<IFormatter>.Method(_Formatter => _Formatter.Serialize(Metadata<Stream>.Value, Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_method.ReturnType),
//                                    _return,
//                                    Expression.Call(_instance, _method, _argument)
//                                ),
//                                _instance,
//                                _argument,
//                                _formatter,
//                                _return
//                            ).Compile();
//                            application.Map(_mapping.Path, _Application => _Application.Run(new Get<T>(container, _mapping.Formatter, _mapping.Manager, _action).Run));
//                        }
//                        else if (_parameter.ParameterType == Metadata<int>.Type)
//                        {
//                            var _argument = Expression.Parameter(Metadata<string>.Type);
//                            var _action = Expression.Lambda<Action<T, string, IFormatter, Stream>>
//                            (
//                                Expression.Block
//                                (
//                                    Expression.IfThen
//                                    (
//                                        Expression.Call(Metadata.Method(() => object.Equals(Metadata<object>.Value, Metadata<object>.Value)), _argument, Expression.Constant(null, Metadata<string>.Type)),
//                                        Expression.Throw(Expression.New(Metadata.Constructor(() => new ArgumentNullException())))
//                                    ),
//                                    Expression.Call
//                                    (
//                                        _formatter,
//                                        Metadata<IFormatter>.Method(_Formatter => _Formatter.Serialize(Metadata<Stream>.Value, Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_method.ReturnType),
//                                        _return,
//                                        Expression.Call
//                                        (
//                                            _instance,
//                                            _method,
//                                            Expression.Call
//                                            (
//                                                Metadata.Method(() => int.Parse(Metadata<string>.Value)),
//                                                _argument
//                                            )
//                                        )
//                                    )
//                                ),
//                                _instance,
//                                _argument,
//                                _formatter,
//                                _return
//                            ).Compile();
//                            application.Map(_mapping.Path, _Application => _Application.Run(new Get<T>(container, _mapping.Formatter, _mapping.Manager, _action).Run));
//                        }
//                        else if (_parameter.ParameterType == Metadata<long>.Type)
//                        {
//                            var _argument = Expression.Parameter(Metadata<string>.Type);
//                            var _action = Expression.Lambda<Action<T, string, IFormatter, Stream>>
//                            (
//                                Expression.Block
//                                (
//                                    Expression.IfThen
//                                    (
//                                        Expression.Call(Metadata.Method(() => object.Equals(Metadata<object>.Value, Metadata<object>.Value)), _argument, Expression.Constant(null, Metadata<string>.Type)),
//                                        Expression.Throw(Expression.New(Metadata.Constructor(() => new ArgumentNullException())))
//                                    ),
//                                    Expression.Call
//                                    (
//                                        _formatter,
//                                        Metadata<IFormatter>.Method(_Formatter => _Formatter.Serialize(Metadata<Stream>.Value, Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_method.ReturnType),
//                                        _return,
//                                        Expression.Call
//                                        (
//                                            _instance,
//                                            _method,
//                                            Expression.Call
//                                            (
//                                                Metadata.Method(() => long.Parse(Metadata<string>.Value)),
//                                                _argument
//                                            )
//                                        )
//                                    )
//                                ),
//                                _instance,
//                                _argument,
//                                _formatter,
//                                _return
//                            ).Compile();
//                            application.Map(_mapping.Path, _Application => _Application.Run(new Get<T>(container, _mapping.Formatter, _mapping.Manager, _action).Run));
//                        }
//                        else if (_parameter.ParameterType == Metadata<short>.Type)
//                        {
//                            var _argument = Expression.Parameter(Metadata<string>.Type);
//                            var _action = Expression.Lambda<Action<T, string, IFormatter, Stream>>
//                            (
//                                Expression.Block
//                                (
//                                    Expression.IfThen
//                                    (
//                                        Expression.Call(Metadata.Method(() => object.Equals(Metadata<object>.Value, Metadata<object>.Value)), _argument, Expression.Constant(null, Metadata<string>.Type)),
//                                        Expression.Throw(Expression.New(Metadata.Constructor(() => new ArgumentNullException())))
//                                    ),
//                                    Expression.Call
//                                    (
//                                        _formatter,
//                                        Metadata<IFormatter>.Method(_Formatter => _Formatter.Serialize(Metadata<Stream>.Value, Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_method.ReturnType),
//                                        _return,
//                                        Expression.Call
//                                        (
//                                            _instance,
//                                            _method,
//                                            Expression.Call
//                                            (
//                                                Metadata.Method(() => short.Parse(Metadata<string>.Value)),
//                                                _argument
//                                            )
//                                        )
//                                    )
//                                ),
//                                _instance,
//                                _argument,
//                                _formatter,
//                                _return
//                            ).Compile();
//                            application.Map(_mapping.Path, _Application => _Application.Run(new Get<T>(container, _mapping.Formatter, _mapping.Manager, _action).Run));
//                        }
//                        else if (_parameter.ParameterType == Metadata<Guid>.Type)
//                        {
//                            var _argument = Expression.Parameter(Metadata<string>.Type);
//                            var _action = Expression.Lambda<Action<T, string, IFormatter, Stream>>
//                            (
//                                Expression.Block
//                                (
//                                    Expression.IfThen
//                                    (
//                                        Expression.Call(Metadata.Method(() => object.Equals(Metadata<object>.Value, Metadata<object>.Value)), _argument, Expression.Constant(null, Metadata<string>.Type)),
//                                        Expression.Throw(Expression.New(Metadata.Constructor(() => new ArgumentNullException())))
//                                    ),
//                                    Expression.Call
//                                    (
//                                        _formatter,
//                                        Metadata<IFormatter>.Method(_Formatter => _Formatter.Serialize(Metadata<Stream>.Value, Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_method.ReturnType),
//                                        _return,
//                                        Expression.Call
//                                        (
//                                            _instance,
//                                            _method,
//                                            Expression.Call
//                                            (
//                                                Metadata.Method(() => Guid.Parse(Metadata<string>.Value)),
//                                                _argument
//                                            )
//                                        )
//                                    )
//                                ),
//                                _instance,
//                                _argument,
//                                _formatter,
//                                _return
//                            ).Compile();
//                            application.Map(_mapping.Path, _Application => _Application.Run(new Get<T>(container, _mapping.Formatter, _mapping.Manager, _action).Run));
//                        }
//                        else if (_parameter.ParameterType == Metadata<uint>.Type)
//                        {
//                            var _argument = Expression.Parameter(Metadata<string>.Type);
//                            var _action = Expression.Lambda<Action<T, string, IFormatter, Stream>>
//                            (
//                                Expression.Block
//                                (
//                                    Expression.IfThen
//                                    (
//                                        Expression.Call(Metadata.Method(() => object.Equals(Metadata<object>.Value, Metadata<object>.Value)), _argument, Expression.Constant(null, Metadata<string>.Type)),
//                                        Expression.Throw(Expression.New(Metadata.Constructor(() => new ArgumentNullException())))
//                                    ),
//                                    Expression.Call
//                                    (
//                                        _formatter,
//                                        Metadata<IFormatter>.Method(_Formatter => _Formatter.Serialize(Metadata<Stream>.Value, Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_method.ReturnType),
//                                        _return,
//                                        Expression.Call
//                                        (
//                                            _instance,
//                                            _method,
//                                            Expression.Call
//                                            (
//                                                Metadata.Method(() => uint.Parse(Metadata<string>.Value)),
//                                                _argument
//                                            )
//                                        )
//                                    )
//                                ),
//                                _instance,
//                                _argument,
//                                _formatter,
//                                _return
//                            ).Compile();
//                            application.Map(_mapping.Path, _Application => _Application.Run(new Get<T>(container, _mapping.Formatter, _mapping.Manager, _action).Run));
//                        }
//                        else if (_parameter.ParameterType == Metadata<ulong>.Type)
//                        {
//                            var _argument = Expression.Parameter(Metadata<string>.Type);
//                            var _action = Expression.Lambda<Action<T, string, IFormatter, Stream>>
//                            (
//                                Expression.Block
//                                (
//                                    Expression.IfThen
//                                    (
//                                        Expression.Call(Metadata.Method(() => object.Equals(Metadata<object>.Value, Metadata<object>.Value)), _argument, Expression.Constant(null, Metadata<string>.Type)),
//                                        Expression.Throw(Expression.New(Metadata.Constructor(() => new ArgumentNullException())))
//                                    ),
//                                    Expression.Call
//                                    (
//                                        _formatter,
//                                        Metadata<IFormatter>.Method(_Formatter => _Formatter.Serialize(Metadata<Stream>.Value, Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_method.ReturnType),
//                                        _return,
//                                        Expression.Call
//                                        (
//                                            _instance,
//                                            _method,
//                                            Expression.Call
//                                            (
//                                                Metadata.Method(() => ulong.Parse(Metadata<string>.Value)),
//                                                _argument
//                                            )
//                                        )
//                                    )
//                                ),
//                                _instance,
//                                _argument,
//                                _formatter,
//                                _return
//                            ).Compile();
//                            application.Map(_mapping.Path, _Application => _Application.Run(new Get<T>(container, _mapping.Formatter, _mapping.Manager, _action).Run));
//                        }
//                        else if (_parameter.ParameterType == Metadata<ushort>.Type)
//                        {
//                            var _argument = Expression.Parameter(Metadata<string>.Type);
//                            var _action = Expression.Lambda<Action<T, string, IFormatter, Stream>>
//                            (
//                                Expression.Block
//                                (
//                                    Expression.IfThen
//                                    (
//                                        Expression.Call(Metadata.Method(() => object.Equals(Metadata<object>.Value, Metadata<object>.Value)), _argument, Expression.Constant(null, Metadata<string>.Type)),
//                                        Expression.Throw(Expression.New(Metadata.Constructor(() => new ArgumentNullException())))
//                                    ),
//                                    Expression.Call
//                                    (
//                                        _formatter,
//                                        Metadata<IFormatter>.Method(_Formatter => _Formatter.Serialize(Metadata<Stream>.Value, Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_method.ReturnType),
//                                        _return,
//                                        Expression.Call
//                                        (
//                                            _instance,
//                                            _method,
//                                            Expression.Call
//                                            (
//                                                Metadata.Method(() => ushort.Parse(Metadata<string>.Value)),
//                                                _argument
//                                            )
//                                        )
//                                    )
//                                ),
//                                _instance,
//                                _argument,
//                                _formatter,
//                                _return
//                            ).Compile();
//                            application.Map(_mapping.Path, _Application => _Application.Run(new Get<T>(container, _mapping.Formatter, _mapping.Manager, _action).Run));
//                        }
//                        else if (_parameter.ParameterType == Metadata<bool>.Type)
//                        {
//                            var _argument = Expression.Parameter(Metadata<string>.Type);
//                            var _action = Expression.Lambda<Action<T, string, IFormatter, Stream>>
//                            (
//                                Expression.Block
//                                (
//                                    Expression.IfThen
//                                    (
//                                        Expression.Call(Metadata.Method(() => object.Equals(Metadata<object>.Value, Metadata<object>.Value)), _argument, Expression.Constant(null, Metadata<string>.Type)),
//                                        Expression.Throw(Expression.New(Metadata.Constructor(() => new ArgumentNullException())))
//                                    ),
//                                    Expression.Call
//                                    (
//                                        _formatter,
//                                        Metadata<IFormatter>.Method(_Formatter => _Formatter.Serialize(Metadata<Stream>.Value, Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_method.ReturnType),
//                                        _return,
//                                        Expression.Call
//                                        (
//                                            _instance,
//                                            _method,
//                                            Expression.Call
//                                            (
//                                                Metadata.Method(() => bool.Parse(Metadata<string>.Value)),
//                                                _argument
//                                            )
//                                        )
//                                    )
//                                ),
//                                _instance,
//                                _argument,
//                                _formatter,
//                                _return
//                            ).Compile();
//                            application.Map(_mapping.Path, _Application => _Application.Run(new Get<T>(container, _mapping.Formatter, _mapping.Manager, _action).Run));
//                        }
//                        else if (_parameter.ParameterType == Metadata<decimal>.Type)
//                        {
//                            var _argument = Expression.Parameter(Metadata<string>.Type);
//                            var _action = Expression.Lambda<Action<T, string, IFormatter, Stream>>
//                            (
//                                Expression.Block
//                                (
//                                    Expression.IfThen
//                                    (
//                                        Expression.Call(Metadata.Method(() => object.Equals(Metadata<object>.Value, Metadata<object>.Value)), _argument, Expression.Constant(null, Metadata<string>.Type)),
//                                        Expression.Throw(Expression.New(Metadata.Constructor(() => new ArgumentNullException())))
//                                    ),
//                                    Expression.Call
//                                    (
//                                        _formatter,
//                                        Metadata<IFormatter>.Method(_Formatter => _Formatter.Serialize(Metadata<Stream>.Value, Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_method.ReturnType),
//                                        _return,
//                                        Expression.Call
//                                        (
//                                            _instance,
//                                            _method,
//                                            Expression.Call
//                                            (
//                                                Metadata.Method(() => decimal.Parse(Metadata<string>.Value)),
//                                                _argument
//                                            )
//                                        )
//                                    )
//                                ),
//                                _instance,
//                                _argument,
//                                _formatter,
//                                _return
//                            ).Compile();
//                            application.Map(_mapping.Path, _Application => _Application.Run(new Get<T>(container, _mapping.Formatter, _mapping.Manager, _action).Run));
//                        }
//                        else if (_parameter.ParameterType == Metadata<DateTime>.Type)
//                        {
//                            var _argument = Expression.Parameter(Metadata<string>.Type);
//                            var _action = Expression.Lambda<Action<T, string, IFormatter, Stream>>
//                            (
//                                Expression.Block
//                                (
//                                    Expression.IfThen
//                                    (
//                                        Expression.Call(Metadata.Method(() => object.Equals(Metadata<object>.Value, Metadata<object>.Value)), _argument, Expression.Constant(null, Metadata<string>.Type)),
//                                        Expression.Throw(Expression.New(Metadata.Constructor(() => new ArgumentNullException())))
//                                    ),
//                                    Expression.Call
//                                    (
//                                        _formatter,
//                                        Metadata<IFormatter>.Method(_Formatter => _Formatter.Serialize(Metadata<Stream>.Value, Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_method.ReturnType),
//                                        _return,
//                                        Expression.Call
//                                        (
//                                            _instance,
//                                            _method,
//                                            Expression.Call
//                                            (
//                                                Metadata.Method(() => DateTime.Parse(Metadata<string>.Value)),
//                                                _argument
//                                            )
//                                        )
//                                    )
//                                ),
//                                _instance,
//                                _argument,
//                                _formatter,
//                                _return
//                            ).Compile();
//                            application.Map(_mapping.Path, _Application => _Application.Run(new Get<T>(container, _mapping.Formatter, _mapping.Manager, _action).Run));
//                        }
//                        else if (_parameter.ParameterType == Metadata<TimeSpan>.Type)
//                        {
//                            var _argument = Expression.Parameter(Metadata<string>.Type);
//                            var _action = Expression.Lambda<Action<T, string, IFormatter, Stream>>
//                            (
//                                Expression.Block
//                                (
//                                    Expression.IfThen
//                                    (
//                                        Expression.Call(Metadata.Method(() => object.Equals(Metadata<object>.Value, Metadata<object>.Value)), _argument, Expression.Constant(null, Metadata<string>.Type)),
//                                        Expression.Throw(Expression.New(Metadata.Constructor(() => new ArgumentNullException())))
//                                    ),
//                                    Expression.Call
//                                    (
//                                        _formatter,
//                                        Metadata<IFormatter>.Method(_Formatter => _Formatter.Serialize(Metadata<Stream>.Value, Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_method.ReturnType),
//                                        _return,
//                                        Expression.Call
//                                        (
//                                            _instance,
//                                            _method,
//                                            Expression.Call
//                                            (
//                                                Metadata.Method(() => TimeSpan.Parse(Metadata<string>.Value)),
//                                                _argument
//                                            )
//                                        )
//                                    )
//                                ),
//                                _instance,
//                                _argument,
//                                _formatter,
//                                _return
//                            ).Compile();
//                            application.Map(_mapping.Path, _Application => _Application.Run(new Get<T>(container, _mapping.Formatter, _mapping.Manager, _action).Run));
//                        }
//                        else if (_parameter.ParameterType == Metadata<Uri>.Type)
//                        {
//                            var _argument = Expression.Parameter(Metadata<string>.Type);
//                            var _action = Expression.Lambda<Action<T, string, IFormatter, Stream>>
//                            (
//                                Expression.Call
//                                (
//                                    _formatter,
//                                    Metadata<IFormatter>.Method(_Formatter => _Formatter.Serialize(Metadata<Stream>.Value, Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_method.ReturnType),
//                                    _return,
//                                    Expression.Call
//                                    (
//                                        _instance,
//                                        _method,
//                                        Expression.New
//                                        (
//                                            Metadata.Constructor(() => new Uri(Metadata<string>.Value)),
//                                            _argument
//                                        )
//                                    )
//                                ),
//                                _instance,
//                                _argument,
//                                _formatter,
//                                _return
//                            ).Compile();
//                            application.Map(_mapping.Path, _Application => _Application.Run(new Get<T>(container, _mapping.Formatter, _mapping.Manager, _action).Run));
//                        }
//                        else if (_parameter.ParameterType == Metadata<IPAddress>.Type)
//                        {
//                            var _argument = Expression.Parameter(Metadata<string>.Type);
//                            var _action = Expression.Lambda<Action<T, string, IFormatter, Stream>>
//                            (
//                                Expression.Call
//                                (
//                                    _formatter,
//                                    Metadata<IFormatter>.Method(_Formatter => _Formatter.Serialize(Metadata<Stream>.Value, Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_method.ReturnType),
//                                    _return,
//                                    Expression.Call
//                                    (
//                                        _instance,
//                                        _method,
//                                        Expression.Call
//                                        (
//                                            Metadata.Method(() => IPAddress.Parse(Metadata<string>.Value)),
//                                            _argument
//                                        )
//                                    )
//                                ),
//                                _instance,
//                                _argument,
//                                _formatter,
//                                _return
//                            ).Compile();
//                            application.Map(_mapping.Path, _Application => _Application.Run(new Get<T>(container, _mapping.Formatter, _mapping.Manager, _action).Run));
//                        }
//                        else { throw new InvalidOperationException("'GET' verb only support a single parameter of one of these types: 'string', 'int', 'long', 'short', 'uint', 'ulong', 'ushort', 'Guid', 'bool', 'decimal', 'DateTime', 'TimeSpan', 'Uri', 'IPAddress'"); }
//                    }
//                    else { throw new InvalidOperationException("'GET' verb only support a single parameter."); }
//                }
//                else
//                {
//                    var _argumentation = new
//                    {
//                        Stream = Expression.Parameter(Metadata<Stream>.Type),
//                        Object = Expression.Parameter(_type),
//                    };
//                    var _action = Expression.Lambda<Action<T, IFormatter, Stream, Stream>>
//                    (
//                        Expression.Block
//                        (
//                            new ParameterExpression[] { _argumentation.Object },
//                            Expression.Assign
//                            (
//                                _argumentation.Object,
//                                Expression.Call
//                                (
//                                    _formatter,
//                                    Metadata<IFormatter>.Method(_Formatter => _Formatter.Deserialize<object>(Metadata<Stream>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_type),
//                                    _argumentation.Stream
//                                )
//                            ),
//                            Expression.Call
//                            (
//                                _formatter,
//                                Metadata<IFormatter>.Method(_Formatter => _Formatter.Serialize(Metadata<Stream>.Value, Metadata<object>.Value)).GetGenericMethodDefinition().MakeGenericMethod(_method.ReturnType),
//                                _return,
//                                Expression.Call
//                                (
//                                    _instance,
//                                    _method,
//                                    _signature.Select(_Parameter => Expression.Field(_argumentation.Object, _Parameter.Name))
//                                )
//                            )
//                        ),
//                        _instance,
//                        _formatter,
//                        _argumentation.Stream,
//                        _return
//                    ).Compile();
//                    application.Map(_mapping.Path, _Application => _Application.Run(Operation.Create<T>(container, _mapping, _action)));
//                }
//            }
//        }

//        static private class Operation
//        {
//            static public RequestDelegate Create<T>(IContainer container, IMapping mapping, Action<T, IFormatter, Stream, Stream> action)
//                where T : class
//            {
//                switch (mapping.Verb)
//                {
//                    case Verbs.POST: return new Post<T>(container, mapping.Formatter, mapping.Manager, action).Run;
//                    case Verbs.PUT: return new Put<T>(container, mapping.Formatter, mapping.Manager, action).Run;
//                    case Verbs.PATCH: return new Patch<T>(container, mapping.Formatter, mapping.Manager, action).Run;
//                    case Verbs.DELETE: return new Delete<T>(container, mapping.Formatter, mapping.Manager, action).Run;
//                    default: throw new NotSupportedException();
//                }
//            }
//        }

//        private class Post<T>
//            where T : class
//        {
//            private IContainer m_Container;
//            private IFormatter m_Formatter;
//            private IManager m_Manager;
//            private Action<T, IFormatter, Stream, Stream> m_Action;

//            public Post(IContainer container, IFormatter formatter, IManager manager, Action<T, IFormatter, Stream, Stream> action)
//            {
//                this.m_Container = container;
//                this.m_Formatter = formatter;
//                this.m_Manager = manager;
//                this.m_Action = action;
//            }

//            public Task Run(HttpContext context)
//            {
//                return Task.Run(() =>
//                {
//                    var _module = null as IModule<T>;
//                    var _response = context.Response;
//                    _response.ContentType = "application/json";
//#if NET452
//                    if (context.Request.HttpMethod == "POST")
//#else
//                    if (context.Request.Method == "POST")
//#endif
//                    {
//                        try
//                        {
//                            _module = this.m_Container.Module<T>();
//#if NET452
//                            this.m_Action(_module.Value, this.m_Formatter, context.Request.InputStream, _response.OutputStream);
//#else
//                            this.m_Action(_module.Value, this.m_Formatter, context.Request.Body, _response.Body);
//#endif
//                        }
//                        catch (Exception exception) { _response.StatusCode = this.m_Manager.Code(exception); }
//                        finally { if (_module != null) { _module.Dispose(); } }
//                    }
//                    else { _response.StatusCode = 405; }
//                });
//            }
//        }

//        private class Put<T>
//            where T : class
//        {
//            private IContainer m_Container;
//            private IFormatter m_Formatter;
//            private IManager m_Manager;
//            private Action<T, IFormatter, Stream, Stream> m_Action;

//            public Put(IContainer container, IFormatter formatter, IManager manager, Action<T, IFormatter, Stream, Stream> action)
//            {
//                this.m_Container = container;
//                this.m_Formatter = formatter;
//                this.m_Manager = manager;
//                this.m_Action = action;
//            }

//            public Task Run(HttpContext context)
//            {
//                return Task.Run(() =>
//                {
//                    var _module = null as IModule<T>;
//                    var _response = context.Response;
//                    _response.ContentType = "application/json";
//#if NET452
//                    if (context.Request.HttpMethod == "PUT")
//#else
//                    if (context.Request.Method == "PUT")
//#endif
//                    {
//                        try
//                        {
//                            _module = this.m_Container.Module<T>();
//#if NET452
//                            this.m_Action(_module.Value, this.m_Formatter, context.Request.InputStream, _response.OutputStream);
//#else
//                            this.m_Action(_module.Value, this.m_Formatter, context.Request.Body, _response.Body);
//#endif
//                        }
//                        catch (Exception exception) { _response.StatusCode = this.m_Manager.Code(exception); }
//                        finally { if (_module != null) { _module.Dispose(); } }
//                    }
//                    else { _response.StatusCode = 405; }
//                });
//            }
//        }

//        private class Patch<T>
//            where T : class
//        {
//            private IContainer m_Container;
//            private IFormatter m_Formatter;
//            private IManager m_Manager;
//            private Action<T, IFormatter, Stream, Stream> m_Action;

//            public Patch(IContainer container, IFormatter formatter, IManager manager, Action<T, IFormatter, Stream, Stream> action)
//            {
//                this.m_Container = container;
//                this.m_Formatter = formatter;
//                this.m_Manager = manager;
//                this.m_Action = action;
//            }

//            public Task Run(HttpContext context)
//            {
//                return Task.Run(() =>
//                {
//                    var _module = null as IModule<T>;
//                    var _response = context.Response;
//                    _response.ContentType = "application/json";
//#if NET452
//                    if (context.Request.HttpMethod == "PATCH")
//#else
//                    if (context.Request.Method == "PATCH")
//#endif
//                    {
//                        try
//                        {
//                            _module = this.m_Container.Module<T>();
//#if NET452
//                            this.m_Action(_module.Value, this.m_Formatter, context.Request.InputStream, _response.OutputStream);
//#else
//                            this.m_Action(_module.Value, this.m_Formatter, context.Request.Body, _response.Body);
//#endif
//                        }
//                        catch (Exception exception) { _response.StatusCode = this.m_Manager.Code(exception); }
//                        finally { if (_module != null) { _module.Dispose(); } }
//                    }
//                    else { _response.StatusCode = 405; }
//                });
//            }
//        }

//        private class Delete<T>
//            where T : class
//        {
//            private IContainer m_Container;
//            private IFormatter m_Formatter;
//            private IManager m_Manager;
//            private Action<T, IFormatter, Stream, Stream> m_Action;

//            public Delete(IContainer container, IFormatter formatter, IManager manager, Action<T, IFormatter, Stream, Stream> action)
//            {
//                this.m_Container = container;
//                this.m_Formatter = formatter;
//                this.m_Manager = manager;
//                this.m_Action = action;
//            }

//            public Task Run(HttpContext context)
//            {
//                return Task.Run(() =>
//                {
//                    var _module = null as IModule<T>;
//                    var _response = context.Response;
//                    _response.ContentType = "application/json";
//#if NET452
//                    if (context.Request.HttpMethod == "DELETE")
//#else
//                    if (context.Request.Method == "DELETE")
//#endif
//                    {
//                        try
//                        {
//                            _module = this.m_Container.Module<T>();
//#if NET452
//                            this.m_Action(_module.Value, this.m_Formatter, context.Request.InputStream, _response.OutputStream);
//#else
//                            this.m_Action(_module.Value, this.m_Formatter, context.Request.Body, _response.Body);
//#endif
//                        }
//                        catch (Exception exception) { _response.StatusCode = this.m_Manager.Code(exception); }
//                        finally { if (_module != null) { _module.Dispose(); } }
//                    }
//                    else { _response.StatusCode = 405; }
//                });
//            }
//        }

//        private class Get<T>
//            where T : class
//        {
//            private IContainer m_Container;
//            private IFormatter m_Formatter;
//            private IManager m_Manager;
//            private Action<T, string, IFormatter, Stream> m_Action;

//            public Get(IContainer container, IFormatter formatter, IManager manager, Action<T, string, IFormatter, Stream> action)
//            {
//                this.m_Container = container;
//                this.m_Formatter = formatter;
//                this.m_Manager = manager;
//                this.m_Action = action;
//            }

//            public Task Run(HttpContext context)
//            {
//                return Task.Run(() =>
//                {
//                    var _path = context.Request.Path;
//                    var _module = null as IModule<T>;
//                    var _response = context.Response;
//                    _response.ContentType = "application/json";
//#if NET452
//                    if (context.Request.HttpMethod == "GET")
//#else
//                    if (context.Request.Method == "GET")
//#endif
//                    {
//                        try
//                        {
//                            _module = this.m_Container.Module<T>();
//#if NET452
//                            this.m_Action(_module.Value, string.IsNullOrWhiteSpace(_path) ? null : _path.Substring(1), this.m_Formatter, _response.OutputStream);
//#else
//                            this.m_Action(_module.Value, _path.HasValue ? _path.Value.Substring(1) : null, this.m_Formatter, _response.Body);
//#endif
//                        }
//                        catch (Exception exception) { _response.StatusCode = this.m_Manager.Code(exception); }
//                        finally { if (_module != null) { _module.Dispose(); } }
//                    }
//                    else { _response.StatusCode = 405; }
//                });
//            }
//        }

//        public class Mapping : Communication.IMapping
//        {
//            private string m_Path;
//            private Communication.Verbs m_Verb;
//            private IFormatter m_Formatter;
//            private Communication.IManager m_Manager;

//            public Mapping(string path, Communication.Verbs verb, IFormatter formatter, Communication.IManager manager)
//            {
//                this.m_Path = path;
//                this.m_Verb = verb;
//                this.m_Formatter = formatter;
//                this.m_Manager = manager;
//            }

//            public string Path
//            {
//                get { return this.m_Path; }
//            }

//            public Communication.Verbs Verb
//            {
//                get { return this.m_Verb; }
//            }

//            public IFormatter Formatter
//            {
//                get { return this.m_Formatter; }
//            }

//            public Communication.IManager Manager
//            {
//                get { return this.m_Manager; }
//            }
//        }
//    }
//}

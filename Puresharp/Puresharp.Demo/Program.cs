using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;
using Puresharp.Legacy;

namespace Puresharp.Legacy
{
    static public class Validation
    {
        static public Advisor Validate<T>(this Advisor.Parameter<T> @this, Action<ParameterInfo, T, string> validate)
            where T : Attribute
        {
            return @this.Visit((_Parameter, _Attribute) => new Validator<T>(_Parameter, _Attribute, validate));
        }
    }

    public class Validator<T> : IVisitor
        where T : Attribute
    {
        private ParameterInfo m_Parameter;
        private T m_Attribute;
        private Action<ParameterInfo, T, string> m_Validate;

        public Validator(ParameterInfo parameter, T attribute, Action<ParameterInfo, T, string> validate)
        {
            this.m_Parameter = parameter;
            this.m_Attribute = attribute;
            this.m_Validate = validate;
        }

        void IVisitor.Visit<T>(Func<T> value)
        {
            var _value = value();
            this.m_Validate(this.m_Parameter, this.m_Attribute, _value == null ? null : _value.ToString());
        }
    }
}

namespace Puresharp.Demo
{
    public interface IHelloWorldService
    {
        string SayHello([EmailAddress] string account);
    }

    public class HelloWorldService : IHelloWorldService
    {
        public string SayHello(string account)
        {
            return $"Hello {account}";
        }
    }
    
    public class Validation : Aspect
    {
        public override IEnumerable<Advisor> Manage(MethodBase method)
        {
            yield return Advice
                .For(method)
                .Parameter<EmailAddressAttribute>()
                .Validate((_Parameter, _Attribute, _Value) =>
                {
                    if (_Value == null) { throw new ArgumentNullException(_Parameter.Name); }
                    try { new MailAddress(_Value.ToString()); }
                    catch (Exception exception) { throw new ArgumentException(_Parameter.Name, exception); }
                });
        }
    }

    public class Demonstration : Aspect
    {
        public override IEnumerable<Advisor> Manage(MethodBase method)
        {
            yield return Advice
                .For(method)
                .Before(invocation =>
                {
                    return Expression.Call
                    (
                        Metadata.Method(() => Console.WriteLine(Metadata<string>.Value)), 
                        Expression.Constant("Hello World")
                    );
                });
        }
    }

    static public class Test
    {
        [Startup]
        static public void Hello()
        {
            var _demonstration = new Demonstration();
            _demonstration.Weave<Pointcut<EmailAddressAttribute>>();

            var _validation = new Validation();
            _validation.Weave<Pointcut<EmailAddressAttribute>>();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var helloWorldService = new HelloWorldService();
            var response1 = helloWorldService.SayHello("tony.thong@outlook.com");
            var response2 = helloWorldService.SayHello("tony");
        }
    }
}
 
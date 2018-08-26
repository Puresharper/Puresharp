using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Reflection;

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

    static public class ValidationExtension
    {
        static public Advisor Validate<TAttribute>(this Advisor.Parameter<TAttribute> @this, Action<ParameterInfo, TAttribute, string> validate)
            where TAttribute : Attribute
        {
            return @this.Supervise().With(_Supervision => new Validator<TAttribute>(_Supervision.Parameter, _Supervision.Attribute, validate));
        }
    }
    
    public class Validator<TAttribute> : ISupervisor
        where TAttribute : Attribute
    {
        private ParameterInfo m_Parameter;
        private TAttribute m_Attribute;
        private Action<ParameterInfo, TAttribute, string> m_Validate;

        public Validator(ParameterInfo parameter, TAttribute attribute, Action<ParameterInfo, TAttribute, string> validate)
        {
            this.m_Parameter = parameter;
            this.m_Attribute = attribute;
            this.m_Validate = validate;
        }

        void ISupervisor.Supervise<T>(T value)
        {
            this.m_Validate(this.m_Parameter, this.m_Attribute, value == null ? null : value.ToString());
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

    static public class Test
    {
        [Startup]
        static public void Hello()
        {
            var validation = new Validation();
            validation.Weave<Pointcut<EmailAddressAttribute>>();
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
 
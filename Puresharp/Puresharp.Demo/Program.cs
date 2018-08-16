using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Puresharp;

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

    //public interface IValidator
    //{
    //    void Validate<T>(ParameterInfo parameter, T value);
    //}

    //public sealed class Validator<TAttribute, TValidator> : Advice, IAdvice
    //    where TAttribute : Attribute
    //    where TValidator : class, IValidator, new()
    //{
    //    private MethodBase m_Method;
    //    private ParameterInfo[] m_Signature;
    //    private TValidator m_Validator;
    //    private int m_Index;

    //    public Validator(MethodBase method)
    //    {
    //        this.m_Method = method;
    //        this.m_Signature = method.GetParameters();
    //        this.m_Validator = new TValidator();
    //        this.m_Index = 0;
    //    }

    //    void IAdvice.Argument<T>(ref T value)
    //    {
    //        var _parameter = this.m_Signature[this.m_Index++];
    //        if (this.Match(_parameter)) { this.m_Validator.Validate(_parameter, value); }
    //    }

    //    private bool Match(ParameterInfo parameter)
    //    {
    //        if (parameter.GetCustomAttributes(typeof(TAttribute), true).Any()) { return true; }
    //        foreach (var _map in this.m_Method.DeclaringType.GetInterfaces().Select(_Interface => this.m_Method.DeclaringType.GetInterfaceMap(_Interface)))
    //        {
    //            for (var _index = 0; _index < _map.TargetMethods.Length; _index++)
    //            {
    //                if (_map.TargetMethods[_index] == this.m_Method)
    //                {
    //                    if (_map.InterfaceMethods[_index].GetParameters().ElementAt(parameter.Position).GetCustomAttributes(typeof(TAttribute), true).Any()) { return true; }
    //                }
    //            }
    //        }
    //        return false;
    //    }
    //}

    public class EmailValidator : IValidator
    {
        public void Validate<T>(ParameterInfo parameter, T value)
        {
            if (value == null) { throw new ArgumentNullException(parameter.Name); }
            new MailAddress(value.ToString());
        }
    }

    public class Validation : Aspect
    {
        public override IEnumerable<Func<IAdvice>> Advise(MethodBase method)
        {
            yield return Validation<EmailAddressAttribute>.With<EmailValidator>(method);
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
 
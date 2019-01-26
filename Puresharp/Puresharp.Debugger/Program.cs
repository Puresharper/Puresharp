using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace Puresharp.Debugger
{
    public class Article
    {
        public string ID { get; set; }
        public int REF { get; set; }
        public DateTime Date { get; set; }
    }

    public interface IArticle
    {
        Article Rechercher(int number);
        int Add(int a, int b);
    }

    public class Article1 : IArticle
    {
        
        private Uri test = new Uri("http://www.micorosoft.com");

        public Uri TEST
        {
            get { return this.test; }
        }

        public int Add(int a, int b)
        {
            return a + b;
        }

        public Article Rechercher(int number)
        {
            return new Article()
            {
                ID = Guid.NewGuid().ToString(),
                REF = 28,
                Date = DateTime.Now
            };
        }
    }

    public class Formatter : IFormatter
    {
        private Newtonsoft.Json.JsonSerializer m_g = Newtonsoft.Json.JsonSerializer.Create();
        private Encoding m_Encoding = new UTF8Encoding(false);

        public T Deserialize<T>(Stream stream)
        {
            try
            {
                //return JsonConvert.DeserializeObject<T>(new StreamReader(stream, this.m_Encoding).ReadToEnd());
                using (var jtr = new JsonTextReader(new StreamReader(stream, this.m_Encoding)))
                {
                    return this.m_g.Deserialize<T>(jtr);
                }
            }
            catch (Exception exception) { throw new ArgumentException("Unable to deserialize json request body.", exception); }
        }

        public void Serialize<T>(Stream stream, T value)
        {
            //var sw = new StreamWriter(stream, this.m_Encoding);
            //var s = JsonConvert.SerializeObject(value);
            //sw.Write(s);
            //sw.Flush();
            var sw = new StreamWriter(stream, this.m_Encoding);
            this.m_g.Serialize(sw, value);
            sw.Flush();
        }
    }

    //public class Manager : Communication.IManager
    //{
    //    public int Code(Exception exception)
    //    {
    //        if (exception is ArgumentException) { return 400; }
    //        if (exception is NotImplementedException) { return 501; }
    //        return 500;
    //    }
    //}

    //public class C1 : Communication.IConvention
    //{
    //    public Communication.IMapping this[MethodInfo method]
    //    {
    //        get
    //        {
    //            var _signature = method.GetParameters();
    //            return new Communication.Mapping
    //            (
    //                $"/{method.DeclaringType.Name}/{method.Name}",
    //                method.ReturnType != Metadata.Void && (_signature.Length > 1 || (_signature.Length > 0 && !_signature.Any(_Parameter => _Parameter.ParameterType == Metadata<string>.Type || _Parameter.ParameterType == Metadata<int>.Type || _Parameter.ParameterType == Metadata<long>.Type || _Parameter.ParameterType == Metadata<short>.Type || _Parameter.ParameterType == Metadata<Guid>.Type || _Parameter.ParameterType == Metadata<uint>.Type || _Parameter.ParameterType == Metadata<ulong>.Type || _Parameter.ParameterType == Metadata<ushort>.Type || _Parameter.ParameterType == Metadata<Uri>.Type || _Parameter.ParameterType == Metadata<IPAddress>.Type || _Parameter.ParameterType == Metadata<DateTime>.Type || _Parameter.ParameterType == Metadata<decimal>.Type || _Parameter.ParameterType == Metadata<TimeSpan>.Type || _Parameter.ParameterType == Metadata<bool>.Type))) ? Communication.Verbs.POST : Communication.Verbs.GET,
    //                new Formatter(),
    //                new Manager()
    //            );
    //        }
    //    }
    //}

    class Program
    {
        static void Main(string[] args)
        {

            var aaa = new Article1() as IArticle;

            //Console.WriteLine("Hello World!");
            var _composition = new Composition()
                .Setup<IArticle>(() => null);
            var _composition2 = new Composition().Setup<IArticle>(() => new Article1());
            _composition.Then(_composition2);
            //var _directory = new Communication.Directory();
            //_directory.Add<IArticle>();
            using (var _container = _composition.Materialize())
            {
                using (var m = _container.Module<IArticle>())
                {
                    m.Value.Add(2, 3);
                }

                //    using (var _communication = new Communication(_container, _directory, new C1()))
                //    {
                //        Console.ReadLine();
                //    }
            }
        }
    }
}

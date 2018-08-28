using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Puresharp
{
    static public class __Advisor
    {
        static public Advisor Around(this Advisor.IGenerator @this, Func<IAdvice> advise)
        {
            return new Advisor(advise);
        }

        //static public Advisor Before(this Advisor.IGenerator @this, Action<ILGenerator> advise)
        //{
        //    //generate implementation of IAdvice where begin method call a static method with the original signature!
        //    //generate dynamic method instead of static!? to manage private types?
        //    throw new NotImplementedException();
        //}

        //static public Advisor Before(this Advisor.IGenerator @this, Func<Advisor.Invocation, Expression> advise)
        //{
        //    //delegate to ilgenerator style!?
        //    throw new NotImplementedException();
        //}

        //static public Advisor.After After(this Advisor.IGenerator @this)
        //{
        //    return new Advisor.After(@this);
        //}

        //static public Advisor After(this Advisor.IGenerator @this, Action<ILGenerator> advise)
        //{
        //    throw new NotImplementedException();
        //}

        //static public Advisor After(this Advisor.IGenerator @this, Func<Advisor.Invocation, Expression> advise)
        //{
        //    throw new NotImplementedException();
        //}

        //static public Advisor Throwing(this Advisor.IAfter @this, Action<ILGenerator> advise)
        //{
        //    throw new NotImplementedException();
        //}

        //static public Advisor Throwing(this Advisor.IAfter @this, Func<Advisor.Execution.Throwing, Expression> advise)
        //{
        //    throw new NotImplementedException();
        //}

        //static public Advisor Returning(this Advisor.IAfter @this, Action<ILGenerator> advise)
        //{
        //    throw new NotImplementedException();
        //}

        //static public Advisor Returning(this Advisor.IAfter @this, Func<Advisor.Execution.Returning, Expression> advise)
        //{
        //    throw new NotImplementedException();
        //}

        static public Advisor.Parameter Parameter(this Advisor.IGenerator @this)
        {
            return new Advisor.Parameter(@this);
        }

        static public Advisor.Parameter<T> Parameter<T>(this Advisor.IGenerator @this)
            where T : Attribute
        {
            return new Advisor.Parameter<T>(@this);
        }
    }
}

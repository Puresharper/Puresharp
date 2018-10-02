using System;

namespace Puresharp
{
    static public class __Composition
    {
        private class Copy : IVisitor
        {
            private IComposition m_Source;
            private IComposition m_Destination;

            public Copy(IComposition source, IComposition destination)
            {
                this.m_Destination = destination;
            }

            public void Visit<T>(Func<T> value)
            {
                if (this.m_Destination.Setup<T>() == null)
                {
                    var _setup = this.m_Source.Setup<T>();
                    this.m_Destination.Setup<T>(_setup.Activation, _setup.Instantiation);
                }
            }
        }

        static public IComposition Then(this IComposition @this, IComposition composition)
        {
            if (@this == null) { return composition; }
            if (composition == null) { return @this; } 
            var _composition = new Composition();
            @this.Accept(new Copy(@this, _composition));
            composition.Accept(new Copy(composition, _composition));
            return _composition;
        }
    }
}

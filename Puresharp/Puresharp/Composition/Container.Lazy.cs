using System;

namespace Puresharp
{
    public partial class Container
    {
        private class Lazy
        {
            static public Func<Resolver, Reservation, object> On(Func<Resolver, Reservation, object> activate)
            {
                var _lazy = new Lazy(activate);
                return new Func<Resolver, Reservation, object>(_lazy.Value);
            }

            private Func<Resolver, Reservation, object> m_Activate;

            public Lazy(Func<Resolver, Reservation, object> activate)
            {
                var _handle = new object();
                var _value = null as object;
                var _activate = activate;
                var _return = new Func<Resolver, Reservation, object>((_Resolver, _Reservation) => _value);
                this.m_Activate = new Func<Resolver, Reservation, object>((_Resolver, _Reservation) =>
                {
                    lock (_handle)
                    {
                        if (_activate == null) { return _value; }
                        _value = _activate(_Resolver, _Reservation);
                        _activate = null;
                        this.m_Activate = _return;
                        return _value;
                    }
                });
            }

            public object Value(Resolver resolver, Reservation reservation)
            {
                return this.m_Activate(resolver, reservation);
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Puresharp
{
    /// <summary>
    /// Container.
    /// </summary>
    internal partial class Container : IContainer
    {
        private Dictionary<Type, Map> m_Mapping = new Dictionary<Type, Map>();
        private Dictionary<Type, Func<Func<Resolver, Reservation, object>>> m_Dictionary = new Dictionary<Type, Func<Func<Resolver, Reservation, object>>>();
        private Reservation m_Reservation;

        /// <summary>
        /// Create a container based on composition.
        /// </summary>
        /// <param name="composition">Composition</param>
        public Container(Composition composition)
        {
            using (var _mapping = new Mapping(composition))
            {
                var _dictionary = new Dictionary<Type, Func<Func<Resolver, Reservation, object>>>();
                var _reservation = new Reservation();
                foreach (var _map in _mapping)
                {
                    var _type = _map.Type;
                    var _activate = _map.Activate;
                    var _instantiation = _map.Instantiation;
                    switch (_map.Instantiation)
                    {
                        case Instantiation.Volatile:
                        {
                            this.m_Mapping.Add(_type, _map);
                            _dictionary.Add(_type, new Func<Func<Resolver, Reservation, object>>(() => new Func<Resolver, Reservation, object>((_Resolver, _Reservation) =>
                            {
                                var _value = _activate(_Resolver, _Reservation);
                                _Reservation.Add(_value);
                                return _value;
                            })));
                            break;
                        }
                        case Instantiation.Multiton:
                        {
                            this.m_Mapping.Add(_type, _map);
                            _dictionary.Add(_type, new Func<Func<Resolver, Reservation, object>>(() =>
                            {
                                var _lazy = new Lazy((_Resolver, _Reservation) =>
                                {
                                    var _value = _activate(_Resolver, _Reservation);
                                    _Reservation.Add(_value);
                                    return _value;
                                });
                                return new Func<Resolver, Reservation, object>(_lazy.Value);
                            }));
                            break;
                        }
                        case Instantiation.Singleton:
                        {
                            this.m_Mapping.Add(_type, _map);
                            var _lazy = new Lazy((_Resolver, _Reservation) =>
                            {
                                var _value = _activate(_Resolver, _Reservation);
                                _reservation.Add(_value);
                                return _value;
                            });
                            _dictionary.Add(_type, new Func<Func<Resolver, Reservation, object>>(() => _lazy.Value));
                            break;
                        }
                    }
                }
                this.m_Dictionary = _dictionary;
                this.m_Reservation = _reservation;
            }
        }
        
        /// <summary>
        /// Instantiate a module.
        /// </summary>
        /// <typeparam name="T">Type of module</typeparam>
        /// <returns>Module</returns>
        public IModule<T> Module<T>()
            where T : class
        {
            var _map = this.m_Mapping[Metadata<T>.Type];
            var _dictionary = new Dictionary<Type, Func<Resolver, Reservation, object>>();
            foreach (var _item in this.m_Dictionary) { _dictionary.Add(_item.Key, _item.Value()); }
            return new Module<T>(_map.Activation as Expression<Func<T>>, _map.Instantiation, new Resolver(_dictionary));
        }

        /// <summary>
        /// Dispose container.
        /// </summary>
        public void Dispose()
        {
            this.m_Reservation.Dispose();
        }
    }
}

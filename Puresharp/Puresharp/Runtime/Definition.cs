using System;
using System.Linq;

namespace Puresharp
{
    public static class Definition<T>
	{
		public static readonly string Value = Definition<T>.Compile();

		private static string Compile()
		{
			var _type = Metadata<T>.Type;
			if (_type.IsGenericType)
			{
				var _field = Metadata.Field<string>(() => Declaration<object>.Value).Name;
				return string.Concat(new string[] { _type.FullName.Substring(0, _type.FullName.IndexOf('[')), "[", string.Join(", ", _type.GetGenericArguments().Select((Type _Argument) => "[" + (typeof(Definition<>).MakeGenericType(new Type[] { _Argument }).GetField(_field).GetValue(null) as string) + "]")), "]", ", ", _type.Assembly.GetName().Name });
			}
			return _type.FullName + ", " + _type.Assembly.GetName().Name;
		}
	}
}

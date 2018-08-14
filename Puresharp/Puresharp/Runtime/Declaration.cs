using System;
using System.Linq;

namespace Puresharp
{
    public static class Declaration<T>
	{
		public static readonly string Value = Declaration<T>.Evaluate();

		static private string Evaluate()
		{
			var type = Metadata<T>.Type;
			if (type.IsGenericType)
			{
				var _Field = Metadata.Field<string>(() => Declaration<object>.Value).Name;
				return type.FullName.Remove(type.FullName.IndexOf('`')) + "<" + string.Join(", ", type.GetGenericArguments().Select(_Argument => typeof(Declaration<>).MakeGenericType(new Type[] { _Argument }).GetField(_Field).GetValue(null) as string)) + ">";
			}
			return type.FullName;
		}
	}
}

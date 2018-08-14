using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Puresharp
{
    public partial class Container
    {
        private class Converter : ExpressionVisitor
        {
            static private Type m_Type = typeof(Metadata<>);
            static private string m_Name = nameof(Metadata<object>.Value);
            static private MethodInfo m_Method = Metadata<IResolver>.Method(_Resolver => _Resolver.Resolve<object>()).GetGenericMethodDefinition();
            private Expression m_Resolver;

            public Converter(Expression resolver)
            {
                this.m_Resolver = resolver;
            }

            override protected Expression VisitMember(MemberExpression node)
            {
                var _member = node.Member;
                if (_member is FieldInfo && _member.DeclaringType.IsGenericType && _member.DeclaringType == Converter.m_Type.MakeGenericType(node.Type) && _member.Name == Converter.m_Name) { return Expression.Call(this.m_Resolver, Converter.m_Method.MakeGenericMethod(node.Type)); }
                return base.VisitMember(node);
            }
        }
    }
}

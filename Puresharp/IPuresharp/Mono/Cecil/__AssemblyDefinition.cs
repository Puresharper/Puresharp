using System;
using System.Linq;
using System.Linq.Expressions;
using Mono;
using Puresharp;

namespace Mono.Cecil
{
    static internal class __AssemblyDefinition
    {
        static public CustomAttribute Attribute<T>(this AssemblyDefinition assembly)
            where T : Attribute
        {
            var _attribute = new CustomAttribute(assembly.MainModule.Import(typeof(T).GetConstructor(Type.EmptyTypes)));
            assembly.CustomAttributes.Add(_attribute);
            return _attribute;
        }

        static public CustomAttribute Attribute<T>(this AssemblyDefinition assembly, Expression<Func<T>> expression)
            where T : Attribute
        {
            var _constructor = (expression.Body as NewExpression).Constructor;
            var _attribute = new CustomAttribute(assembly.MainModule.Import(_constructor));
            foreach (var _argument in (expression.Body as NewExpression).Arguments) { _attribute.ConstructorArguments.Add(new CustomAttributeArgument(assembly.MainModule.Import(_argument.Type), Expression.Lambda<Func<object>>(Expression.Convert(_argument, Metadata<object>.Type)).Compile()())); }
            assembly.CustomAttributes.Add(_attribute);
            return _attribute;
        }
    }
}

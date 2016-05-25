using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NServiceBusTools.Linq.Expressions;

namespace NServiceBusTools.Reflection
{
    public static class Reflect
    {
        public static MethodInfo Method(Expression<Action> methodSelector)
        {
            return methodSelector.AsEnumerable().OfType<MethodCallExpression>().Single().Method;
        }
        public static MethodInfo Method<TResult>(Expression<Func<TResult>> methodSelector)
        {
            return methodSelector.AsEnumerable().OfType<MethodCallExpression>().Single().Method;
        }

        public static PropertyInfo Property<TResult>(Expression<Func<TResult>> propertySelector)
        {
            return (PropertyInfo) propertySelector.AsEnumerable().OfType<MemberExpression>().Single().Member;
        }
    }

    public static class Reflect<T>
    {
        public static MethodInfo Method<TResult>(Expression<Func<T, TResult>> methodSelector)
        {
            return methodSelector.AsEnumerable().OfType<MethodCallExpression>().Single().Method;
        }
    }
}

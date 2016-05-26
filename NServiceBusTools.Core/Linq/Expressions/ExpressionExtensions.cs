using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NServiceBusTools.Linq.Expressions
{
    public static class ExpressionExtensions
    {
        public static IEnumerable<Expression> AsEnumerable(this Expression expression)
        {
            var searcher = new ExpressionSearchVistor(x => true);
            searcher.Visit(expression);
            return searcher.Expressions;
        }

        private sealed class ExpressionSearchVistor : ExpressionVisitor
        {
            private readonly Func<Expression, bool> _predicate;
            public HashSet<Expression> Expressions { get; private set; } 

            public ExpressionSearchVistor(Func<Expression, bool> predicate)
            {
                _predicate = predicate;
                Expressions = new HashSet<Expression>();
            }

            public override Expression Visit(Expression node)
            {
                if (_predicate(node))
                {
                    Expressions.Add(node);
                }
                return base.Visit(node);
            }
        }
    }
}

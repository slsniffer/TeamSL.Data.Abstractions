using System;
using System.Linq.Expressions;

namespace TeamSL.Data.Abstractions.Filtering
{
    public abstract class ExpressionQuerySpecification<TRecord> : QuerySpecification<TRecord>
    {
        private readonly Expression<Func<TRecord, bool>> _expression;

        protected ExpressionQuerySpecification(Expression<Func<TRecord, bool>> expression)
        {
            Checks.NotNull(expression, nameof(expression));
            _expression = expression;
        }

        protected override Expression<Func<TRecord, bool>> MatchingCriteria()
        {
            return _expression;
        }
    }
}

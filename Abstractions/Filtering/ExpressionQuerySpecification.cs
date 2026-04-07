using System;
using System.Linq.Expressions;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Filtering
{
    public abstract class ExpressionQuerySpecification<TRecord, TKey> : QuerySpecification<TRecord>
        where TRecord : IRecord
    {
        private readonly Expression<Func<TRecord, bool>> _expression;

        protected ExpressionQuerySpecification(Expression<Func<TRecord, bool>> expression)
        {
            _expression = expression;
        }

        protected override Expression<Func<TRecord, bool>> MatchingCriteria()
        {
            return _expression;
        }
    }
}
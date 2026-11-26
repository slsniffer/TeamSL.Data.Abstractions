using System;
using System.Linq;
using System.Linq.Expressions;

namespace TeamSL.Data.Abstractions.Filtering
{
    public abstract class QuerySpecification<TRecord> : IQuerySpecification<TRecord>
    {
        protected abstract Expression<Func<TRecord, bool>>? MatchingCriteria();

        public virtual IQueryable<TRecord> Satisfy(IQueryable<TRecord> candidates)
        {
            var criteria = MatchingCriteria();
            return criteria != null ? candidates.Where(criteria) : candidates;
        }
    }
}

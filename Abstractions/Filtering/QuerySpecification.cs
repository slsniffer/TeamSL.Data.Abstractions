using System;
using System.Linq;
using System.Linq.Expressions;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Filtering
{
    public abstract class QuerySpecification<TRecord> : IQuerySpecification<TRecord>
        where TRecord : IRecord
    {
        protected abstract Expression<Func<TRecord, bool>> MatchingCriteria();

        public virtual IQueryable<TRecord> Satisfy(IQueryable<TRecord> candidates)
        {
            if (MatchingCriteria() != null)
            {
                return candidates.Where(MatchingCriteria());
            }

            return candidates;
        }
    }
}

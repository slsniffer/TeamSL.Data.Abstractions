using System;
using System.Linq;
using System.Linq.Expressions;

namespace TeamSL.Data.Abstractions.Filtering
{
    public abstract class QuerySpecification<T> : IQuerySpecification<T>
    {
        protected abstract Expression<Func<T, bool>> MatchingCriteria();

        public virtual IQueryable<T> Satisfy(IQueryable<T> candidates)
        {
            if (MatchingCriteria() != null)
            {
                return candidates.Where(MatchingCriteria());
            }

            return candidates;
        }
    }
}

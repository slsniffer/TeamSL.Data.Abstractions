using System.Linq;

namespace TeamSL.Data.Abstractions.Filtering
{
    public static class FilteringExtensions
    {
        public static IQueryable<TRecord> FilterBy<TRecord>(this IQueryable<TRecord> source, IQuerySpecification<TRecord> specification)
        {
            Checks.NotNull(source, nameof(source));
            Checks.NotNull(specification, nameof(specification));
            return specification.Satisfy(source);
        }

        public static IQueryable<TRecord> PageBy<TRecord>(this IQueryable<TRecord> queryable, int skip, int take)
        {
            Checks.NotNull(queryable, nameof(queryable));
            return queryable.Skip(skip).Take(take);
        }
    }
}

using System.Linq;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Filtering
{
    public static class FilteringExtensions
    {
        public static IQueryable<TRecord> FilterBy<TRecord, TKey>(this IQueryable<TRecord> source, IQuerySpecification<TRecord, TKey> specification)
            where TRecord : IRecord<TKey>
        {
            Checks.NotNull(source, nameof(source));
            return specification.Satisfy(source);
        }

        public static IQueryable<TRecord> PageBy<TRecord, TKey>(this IQueryable<TRecord> queryable, int skip, int take)
            where TRecord : IRecord<TKey>
        {
            Checks.NotNull(queryable, nameof(queryable));
            return queryable.Skip(skip).Take(take);
        }
    }
}
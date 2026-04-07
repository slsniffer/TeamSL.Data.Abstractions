using System.Linq;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Fetching
{
    public static class FetchingExtensions
    {
        public static IQueryable<TRecord> Fetch<TRecord, TKey>(this IQueryable<TRecord> source, IFetchStrategy<TRecord, TKey> fetchStrategy)
            where TRecord : IRecord<TKey>
        {
            Checks.NotNull(source, nameof(source));
            return fetchStrategy.Apply(source);
        }
    }
}

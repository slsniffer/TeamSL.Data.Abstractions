using System.Linq;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Fetching
{
    public static class FetchingExtensions
    {
        public static IQueryable<TRecord> Fetch<TRecord>(this IQueryable<TRecord> source, IFetchStrategy<TRecord> fetchStrategy) where TRecord : Record
        {
            Checks.NotNull(source, nameof(source));

            return fetchStrategy.Apply(source);
        }
    }
}

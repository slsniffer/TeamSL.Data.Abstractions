using System.Linq;

namespace TeamSL.Data.Abstractions.Fetching
{
    public static class FetchingExtensions
    {
        public static IQueryable<TRecord> Fetch<TRecord>(this IQueryable<TRecord> source, IFetchStrategy<TRecord> fetchStrategy)
        {
            Checks.NotNull(source, nameof(source));
            Checks.NotNull(fetchStrategy, nameof(fetchStrategy));
            return fetchStrategy.Apply(source);
        }
    }
}

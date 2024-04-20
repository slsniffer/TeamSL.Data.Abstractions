using System;
using System.Linq;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Ordering
{
    public static class OrderingExtensions
    {
        public static IQueryable<TRecord> OrderBy<TRecord>(this IQueryable<TRecord> queryable, Action<Orderable<TRecord>> order) where TRecord : Record
        {
            Checks.NotNull(queryable, nameof(queryable));

            var orderable = new Orderable<TRecord>(queryable);
            order(orderable);
            return orderable.Queryable;
        }
    }
}

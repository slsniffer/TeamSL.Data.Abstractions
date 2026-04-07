using System;
using System.Linq;

namespace TeamSL.Data.Abstractions.Ordering
{
    public static class OrderingExtensions
    {
        public static IQueryable<TRecord> OrderBy<TRecord>(this IQueryable<TRecord> queryable, Action<Orderable<TRecord>> order)
        {
            var orderable = new Orderable<TRecord>(queryable);
            order(orderable);
            return orderable.Queryable;
        }
    }
}

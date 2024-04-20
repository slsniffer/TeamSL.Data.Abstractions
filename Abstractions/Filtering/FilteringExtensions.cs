using System.Linq;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Filtering
{
    public static class FilteringExtensions
    {
        public static ISpecification<TRecord> And<TRecord>(this ISpecification<TRecord> left, ISpecification<TRecord> right)
            where TRecord : Record
        {
            return new AndSpecification<TRecord>(left, right);
        }

        public static ISpecification<TRecord> Or<TRecord>(this ISpecification<TRecord> left, ISpecification<TRecord> right)
            where TRecord : Record
        {
            return new OrSpecification<TRecord>(left, right);
        }

        public static ISpecification<TRecord> Not<TRecord>(this ISpecification<TRecord> spec)
            where TRecord : Record
        {
            return new NotSpecification<TRecord>(spec);
        }

        public static IQueryable<TRecord> FilterBy<TRecord>(this IQueryable<TRecord> source, IQuerySpecification<TRecord> specification) where TRecord : Record
        {
            Checks.NotNull(source, nameof(source));

            return specification.Satisfy(source);
        }

        public static IQueryable<T> PageBy<T>(this IQueryable<T> queryable, int skip, int take)
        {
            Checks.NotNull(queryable, nameof(queryable));

            return queryable.Skip(skip).Take(take);
        }
    }
}
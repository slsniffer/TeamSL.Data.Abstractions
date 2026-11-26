using System;
using System.Linq;
using System.Linq.Expressions;

namespace TeamSL.Data.Abstractions.Ordering
{
    /// <summary>
    /// Fluent ordering builder. The first call sets the primary sort; every subsequent call
    /// adds a secondary sort (ThenBy / ThenByDescending), so <c>x.Asc(a).Desc(b)</c> orders by
    /// <c>a</c> ascending, then by <c>b</c> descending.
    /// </summary>
    public class Orderable<TRecord>
    {
        private IQueryable<TRecord> _queryable;
        private IOrderedQueryable<TRecord>? _ordered;

        public Orderable(IQueryable<TRecord> queryable)
        {
            Checks.NotNull(queryable, nameof(queryable));

            _queryable = queryable;
        }

        public IQueryable<TRecord> Queryable => _queryable;

        public Orderable<TRecord> Asc<TKey>(Expression<Func<TRecord, TKey>> keySelector)
        {
            Checks.NotNull(keySelector, nameof(keySelector));

            _ordered = _ordered == null
                ? _queryable.OrderBy(keySelector)
                : _ordered.ThenBy(keySelector);
            _queryable = _ordered;
            return this;
        }

        public Orderable<TRecord> Asc<TKey1, TKey2>(Expression<Func<TRecord, TKey1>> keySelector1,
            Expression<Func<TRecord, TKey2>> keySelector2)
        {
            return Asc(keySelector1).Asc(keySelector2);
        }

        public Orderable<TRecord> Asc<TKey1, TKey2, TKey3>(Expression<Func<TRecord, TKey1>> keySelector1,
            Expression<Func<TRecord, TKey2>> keySelector2,
            Expression<Func<TRecord, TKey3>> keySelector3)
        {
            return Asc(keySelector1).Asc(keySelector2).Asc(keySelector3);
        }

        public Orderable<TRecord> Desc<TKey>(Expression<Func<TRecord, TKey>> keySelector)
        {
            Checks.NotNull(keySelector, nameof(keySelector));

            _ordered = _ordered == null
                ? _queryable.OrderByDescending(keySelector)
                : _ordered.ThenByDescending(keySelector);
            _queryable = _ordered;
            return this;
        }

        public Orderable<TRecord> Desc<TKey1, TKey2>(Expression<Func<TRecord, TKey1>> keySelector1,
            Expression<Func<TRecord, TKey2>> keySelector2)
        {
            return Desc(keySelector1).Desc(keySelector2);
        }

        public Orderable<TRecord> Desc<TKey1, TKey2, TKey3>(Expression<Func<TRecord, TKey1>> keySelector1,
            Expression<Func<TRecord, TKey2>> keySelector2,
            Expression<Func<TRecord, TKey3>> keySelector3)
        {
            return Desc(keySelector1).Desc(keySelector2).Desc(keySelector3);
        }
    }
}

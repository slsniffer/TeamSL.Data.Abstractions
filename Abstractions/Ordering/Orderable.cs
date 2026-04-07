using System;
using System.Linq;
using System.Linq.Expressions;

namespace TeamSL.Data.Abstractions.Ordering
{
    public class Orderable<TRecord>
    {
        private IQueryable<TRecord> _queryable;

        public Orderable(IQueryable<TRecord> queryable)
        {
            Checks.NotNull(queryable, nameof(queryable));

            _queryable = queryable;
        }

        public IQueryable<TRecord> Queryable => _queryable;

        public Orderable<TRecord> Asc<TKey>(Expression<Func<TRecord, TKey>> keySelector)
        {
            _queryable = _queryable
                .OrderBy(keySelector);
            return this;
        }

        public Orderable<TRecord> Asc<TKey1, TKey2>(Expression<Func<TRecord, TKey1>> keySelector1,
            Expression<Func<TRecord, TKey2>> keySelector2)
        {
            _queryable = _queryable
                .OrderBy(keySelector1)
                .ThenBy(keySelector2);
            return this;
        }

        public Orderable<TRecord> Asc<TKey1, TKey2, TKey3>(Expression<Func<TRecord, TKey1>> keySelector1,
            Expression<Func<TRecord, TKey2>> keySelector2,
            Expression<Func<TRecord, TKey3>> keySelector3)
        {
            _queryable = _queryable
                .OrderBy(keySelector1)
                .ThenBy(keySelector2)
                .ThenBy(keySelector3);
            return this;
        }

        public Orderable<TRecord> Desc<TKey>(Expression<Func<TRecord, TKey>> keySelector)
        {
            _queryable = _queryable
                .OrderByDescending(keySelector);
            return this;
        }

        public Orderable<TRecord> Desc<TKey1, TKey2>(Expression<Func<TRecord, TKey1>> keySelector1,
            Expression<Func<TRecord, TKey2>> keySelector2)
        {
            _queryable = _queryable
                .OrderByDescending(keySelector1)
                .ThenByDescending(keySelector2);
            return this;
        }

        public Orderable<TRecord> Desc<TKey1, TKey2, TKey3>(Expression<Func<TRecord, TKey1>> keySelector1,
            Expression<Func<TRecord, TKey2>> keySelector2,
            Expression<Func<TRecord, TKey3>> keySelector3)
        {
            _queryable = _queryable
                .OrderByDescending(keySelector1)
                .ThenByDescending(keySelector2)
                .ThenByDescending(keySelector3);
            return this;
        }
    }
}
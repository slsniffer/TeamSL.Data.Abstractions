using System;
using System.Linq.Expressions;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Filtering
{
    /// <summary>
    /// Matches the record with the given <see cref="IRecord{TKey}.Id"/>.
    /// Use with Find / Exists / Count, e.g. <c>repository.Find(new ById&lt;Order, Guid&gt;(id))</c>.
    /// </summary>
    public class ById<TRecord, TKey> : QuerySpecification<TRecord>
        where TRecord : IRecord<TKey>
    {
        private readonly TKey _id;

        public ById(TKey id) => _id = id;

        // Reading the value from a field of this instance makes EF send it as a query parameter, not a SQL literal.
        protected override Expression<Func<TRecord, bool>> MatchingCriteria() => x => x.Id!.Equals(_id);
    }

    /// <summary>Matches the record with the given <c>long</c> Id.</summary>
    public class ById<TRecord> : ById<TRecord, long>
        where TRecord : IRecord
    {
        public ById(long id) : base(id)
        {
        }
    }
}

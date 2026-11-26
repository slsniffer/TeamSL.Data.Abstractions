using System;
using System.Linq.Expressions;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Filtering
{
    /// <summary>
    /// Matches the record with the given <see cref="IPublicRecord{TKey}.PublicId"/>.
    /// Use with Find / Exists / Count, e.g. <c>repository.Find(new ByPublicId&lt;Order, Guid&gt;(publicId))</c>.
    /// </summary>
    public class ByPublicId<TRecord, TKey> : QuerySpecification<TRecord>
        where TRecord : IPublicRecord<TKey>
    {
        private readonly Guid _publicId;

        public ByPublicId(Guid publicId) => _publicId = publicId;

        // Reading the value from a field of this instance makes EF send it as a query parameter, not a SQL literal.
        protected override Expression<Func<TRecord, bool>> MatchingCriteria() => x => x.PublicId == _publicId;
    }

    /// <summary>Matches the record with the given PublicId, for records with a <c>long</c> Id.</summary>
    public class ByPublicId<TRecord> : ByPublicId<TRecord, long>
        where TRecord : IPublicRecord
    {
        public ByPublicId(Guid publicId) : base(publicId)
        {
        }
    }
}

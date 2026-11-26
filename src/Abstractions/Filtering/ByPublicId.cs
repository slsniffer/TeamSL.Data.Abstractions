using System;
using System.Linq.Expressions;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Filtering
{
    /// <summary>
    /// Matches the record with the given <see cref="IPublicRecord{TPublicKey, TInnerKey}.PublicId"/>.
    /// Use with Find / Exists / Count, e.g. <c>repository.Find(new ByPublicId&lt;Order, Guid, Guid&gt;(publicId))</c>.
    /// </summary>
    public class ByPublicId<TRecord, TPublicKey, TInnerKey> : QuerySpecification<TRecord>
        where TRecord : IPublicRecord<TPublicKey, TInnerKey>
    {
        private readonly TPublicKey _publicId;

        public ByPublicId(TPublicKey publicId) => _publicId = publicId;

        // Reading the value from a field of this instance makes EF send it as a query parameter, not a SQL literal.
        protected override Expression<Func<TRecord, bool>> MatchingCriteria() => x => x.PublicId!.Equals(_publicId);
    }

    /// <summary>Matches the record with the given PublicId, for records with a <c>long</c> internal Id.</summary>
    public class ByPublicId<TRecord, TPublicKey> : ByPublicId<TRecord, TPublicKey, long>
        where TRecord : IPublicRecord<TPublicKey>
    {
        public ByPublicId(TPublicKey publicId) : base(publicId)
        {
        }
    }

    /// <summary>Matches the record with the given <see cref="Guid"/> PublicId, for records with a <c>long</c> internal Id.</summary>
    public class ByPublicId<TRecord> : ByPublicId<TRecord, Guid>
        where TRecord : IPublicRecord
    {
        public ByPublicId(Guid publicId) : base(publicId)
        {
        }
    }
}

using System.Linq;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Filtering
{
    public class ByIdQuerySpecification<TRecord, TKey> : IQuerySpecification<TRecord, TKey>
        where TRecord : IRecord<TKey>
    {
        private readonly TKey _recordId;

        public ByIdQuerySpecification(TKey recordId)
        {
            _recordId = recordId;
        }

        public virtual IQueryable<TRecord> Satisfy(IQueryable<TRecord> candidates)
        {
            return candidates.Where(x => x.Id!.Equals(_recordId));
        }
    }
}
using System.Linq;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Filtering
{
    public class ByIdQuerySpecification<TRecord> : IQuerySpecification<TRecord>
        where TRecord : Record
    {
        private readonly long _recordId;

        public ByIdQuerySpecification(long recordId)
        {
            _recordId = recordId;
        }

        public virtual IQueryable<TRecord> Satisfy(IQueryable<TRecord> candidates)
        {
            return candidates.Where(x => x.Id == _recordId);
        }
    }
}
using System.Threading;
using System.Threading.Tasks;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Repository
{
    public interface IRepository<in TRecord, TKey>
        where TRecord : IRecord<TKey>
    {
        Task Create(TRecord record, CancellationToken cancellationToken = default);
        Task Update(TRecord record, CancellationToken cancellationToken = default);
        Task Delete(TRecord record, CancellationToken cancellationToken = default);
    }

    public interface IRepository<in TRecord> : IRepository<TRecord, long>
        where TRecord : IRecord
    {
    }
}
using System.Threading;
using System.Threading.Tasks;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Repository
{
    public interface IRepository<in TRecord> where TRecord : Record
    {
        Task Create(TRecord record, CancellationToken cancellationToken = default);
        Task Update(TRecord record, CancellationToken cancellationToken = default);
        Task Delete(TRecord record, CancellationToken cancellationToken = default);
    }
}
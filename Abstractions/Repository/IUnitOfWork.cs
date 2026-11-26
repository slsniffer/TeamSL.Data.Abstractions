using System.Threading;
using System.Threading.Tasks;

namespace TeamSL.Data.Abstractions.Repository
{
    /// <summary>
    /// Stages changes and commits them. <see cref="Add{TRecord}"/>, <see cref="Update{TRecord}"/> and
    /// <see cref="Remove{TRecord}"/> only stage; nothing is persisted until <see cref="SaveChanges"/>,
    /// which writes everything staged in the session, plus edits to records loaded through
    /// <see cref="IReadRepository{TRecord}"/>, in one transaction.
    /// </summary>
    public interface IUnitOfWork
    {
        void Add<TRecord>(TRecord record) where TRecord : class;

        /// <summary>
        /// Marks a detached record (e.g. built from a DTO) as modified.
        /// Records loaded through <see cref="IReadRepository{TRecord}"/> are tracked and saved without calling this.
        /// </summary>
        void Update<TRecord>(TRecord record) where TRecord : class;

        void Remove<TRecord>(TRecord record) where TRecord : class;

        /// <returns>The number of records written.</returns>
        Task<int> SaveChanges(CancellationToken cancellationToken = default);
    }
}

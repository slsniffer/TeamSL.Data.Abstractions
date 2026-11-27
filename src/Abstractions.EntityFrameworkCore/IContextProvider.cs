using Microsoft.EntityFrameworkCore;

namespace TeamSL.Data.Abstractions.EntityFrameworkCore;

/// <summary>
/// Supplies the DbContext(s) used by the repositories and the unit of work.
/// An application may use several DbContexts (e.g. one per schema); each entity type is mapped by exactly one of them.
/// </summary>
public interface IContextProvider
{
    /// <summary>The DbContext that maps <typeparamref name="TRecord"/>.</summary>
    DbContext Context<TRecord>() where TRecord : class;

    /// <summary>All DbContexts of the scope; <see cref="IUnitOfWork.SaveChanges"/> saves each of them.</summary>
    IEnumerable<DbContext> Contexts();
}

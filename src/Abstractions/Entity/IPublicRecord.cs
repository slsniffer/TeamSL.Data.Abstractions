using System;

namespace TeamSL.Data.Abstractions.Entity
{
    /// <summary>A record with an internal <see cref="IRecord{TKey}.Id"/> and a separate <see cref="PublicId"/> that is safe to expose externally.</summary>
    /// <typeparam name="TPublicKey">Type of <see cref="PublicId"/>.</typeparam>
    /// <typeparam name="TInnerKey">Type of the internal Id.</typeparam>
    public interface IPublicRecord<TPublicKey, TInnerKey> : IRecord<TInnerKey>
    {
        TPublicKey PublicId { get; set; }
    }

    /// <summary>A public record with a <c>long</c> internal Id.</summary>
    /// <typeparam name="TPublicKey">Type of PublicId.</typeparam>
    public interface IPublicRecord<TPublicKey> : IPublicRecord<TPublicKey, long>, IRecord
    {
    }

    /// <summary>The common case: a <c>long</c> internal Id and a <see cref="Guid"/> PublicId.</summary>
    public interface IPublicRecord : IPublicRecord<Guid>
    {
    }
}

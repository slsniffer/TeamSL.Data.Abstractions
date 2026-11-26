using System;

namespace TeamSL.Data.Abstractions.Entity
{
    /// <summary>A record with an internal <see cref="IRecord{TKey}.Id"/> and a separate identifier that is safe to expose externally.</summary>
    public interface IPublicRecord<TKey> : IRecord<TKey>
    {
        Guid PublicId { get; set; }
    }

    /// <summary>A public record with a <c>long</c> Id.</summary>
    public interface IPublicRecord : IPublicRecord<long>, IRecord
    {
    }
}

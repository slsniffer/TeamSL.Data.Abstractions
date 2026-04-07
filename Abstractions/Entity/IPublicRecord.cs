using System;

namespace TeamSL.Data.Abstractions.Entity
{
    public interface IPublicRecord : IRecord
    {
        Guid PublicId { get; set; }
    }

    public interface IPublicRecord<TKey> : IRecord<TKey>
    {
        Guid PublicId { get; set; }
    }
}
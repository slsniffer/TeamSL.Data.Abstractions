namespace TeamSL.Data.Abstractions.Entity
{
    public interface IRecord : IRecord<long>
    {
    }

    public interface IRecord<TKey>
    {
        TKey Id { get; set; }
    }
}
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Filtering
{
    public interface ISpecification<in TRecord>
        where TRecord : Record
    {
        bool IsSatisfiedBy(TRecord record);
    }
}
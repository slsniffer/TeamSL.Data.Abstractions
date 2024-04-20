using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Filtering
{
    internal class AndSpecification<TRecord> : ISpecification<TRecord>
        where TRecord : Record
    {
        private readonly ISpecification<TRecord> _left;
        private readonly ISpecification<TRecord> _right;

        internal AndSpecification(ISpecification<TRecord> left, ISpecification<TRecord> right)
        {
            Checks.NotNull(left, nameof(left));
            Checks.NotNull(right, nameof(right));

            _left = left;
            _right = right;
        }

        public bool IsSatisfiedBy(TRecord candidate)
        {
            return _left.IsSatisfiedBy(candidate) && _right.IsSatisfiedBy(candidate);
        }
    }
}
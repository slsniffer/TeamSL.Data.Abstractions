using System;
using TeamSL.Data.Abstractions.Entity;

namespace TeamSL.Data.Abstractions.Filtering
{
    internal class OrSpecification<TRecord> : ISpecification<TRecord>
        where TRecord : Record
    {
        private readonly ISpecification<TRecord> _left;
        private readonly ISpecification<TRecord> _right;

        protected ISpecification<TRecord> Left => _left;

        protected ISpecification<TRecord> Right => _right;

        internal OrSpecification(ISpecification<TRecord> left, ISpecification<TRecord> right)
        {
            if (left == null)
                throw new ArgumentNullException("left");

            if (right == null)
                throw new ArgumentNullException("right");

            _left = left;
            _right = right;
        }

        public bool IsSatisfiedBy(TRecord candidate)
        {
            return Left.IsSatisfiedBy(candidate) || Right.IsSatisfiedBy(candidate);
        }
    }
}
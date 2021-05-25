namespace Authorizer.Domain.Specs.Base
{
    public class OrSpecification<T> : CompositeSpecification<T>
    {
        private readonly ISpecification<T> left;
        private readonly ISpecification<T> right;

        public OrSpecification(ISpecification<T> left, ISpecification<T> right)
            => (this.left, this.right) = (left, right);

        public override bool IsSatisfiedBy(T candidate)
        {
            return this.left.IsSatisfiedBy(candidate) || this.right.IsSatisfiedBy(candidate);
        }
    }
}
namespace Authorizer.Domain.Specs.Base
{
    public class AndSpecification<T> : CompositeSpecification<T>
    {
        private readonly ISpecification<T> left;
        private readonly ISpecification<T> right;
        
        public AndSpecification(ISpecification<T> left, ISpecification<T> right)
            => (this.left, this.right) = (left, right);

        public override bool IsSatisfiedBy(T candidate)
        {
            return this.left.IsSatisfiedBy(candidate) && this.right.IsSatisfiedBy(candidate);
        }
    }
}
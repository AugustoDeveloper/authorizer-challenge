using Authorizer.Domain.Entities;
using Authorizer.Domain.Repositories;
using Authorizer.Domain.Specs.Base;

namespace Authorizer.Domain.Specs
{
    public class AccountHasSufficientLimitToAuthorizationSpecification : CompositeSpecification<Transaction>
    {
        private readonly Account currentAccount;
        private readonly Violations violations;

        public AccountHasSufficientLimitToAuthorizationSpecification(Account currentAccount, Violations violations)
            => (this.currentAccount, this.violations) = (currentAccount, violations);

        public override bool IsSatisfiedBy(Transaction candidate)
        {
            if (this.currentAccount.AvailableLimit < candidate.Amount)
            {
                violations.CurrentAccount = currentAccount;
                violations.Add(Violation.InsuficientLimit);
                return false;
            }

            return true;
        }
    }
}
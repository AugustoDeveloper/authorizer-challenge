using Authorizer.Domain.Entities;
using Authorizer.Domain.Repositories;
using Authorizer.Domain.Specs.Base;

namespace Authorizer.Domain.Specs
{
    public class AccountCreatedToAuthorizationSpecification : CompositeSpecification<Transaction>
    {
        private readonly Account currentAccount;
        private readonly Violations violations;

        public AccountCreatedToAuthorizationSpecification(Account currentAccount, Violations violations)
            => (this.currentAccount, this.violations) = (currentAccount, violations);

        public override bool IsSatisfiedBy(Transaction candidate)
        {
            if (this.currentAccount == null)
            {
                violations.CurrentAccount = this.currentAccount;
                violations.Add(Violation.AccountNotInitialized);
                return false;
            }

            return true;
        }
    }
}
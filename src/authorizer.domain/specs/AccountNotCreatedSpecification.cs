using Authorizer.Domain.Entities;
using Authorizer.Domain.Specs.Base;

namespace Authorizer.Domain.Specs
{
    public class AccountNotCreatedSpecification : CompositeSpecification<Account>
    {
        private readonly Account currentAccount;
        private readonly Violations violations;

        public AccountNotCreatedSpecification(Account currentAccount, Violations violations)
            => (this.currentAccount, this.violations) = (currentAccount, violations);

        public override bool IsSatisfiedBy(Account candidate)
        {
            if (currentAccount != null)
            {
                violations.CurrentAccount = currentAccount;
                violations.Add(Violation.AccountAlreadyInitialized);
                return false;
            }

            return true;
        }
    }
}
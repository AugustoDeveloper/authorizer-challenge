using System;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Repositories;
using Authorizer.Domain.Specs.Base;

namespace Authorizer.Domain.Specs
{
    public class AccountCreatedToAllowListSpecification : CompositeSpecification<AccountAllowList>
    {
        private readonly Account currentAccount;
        private readonly Violations violations;

        public AccountCreatedToAllowListSpecification(Account currentAccount, Violations violations)
        {
            this.violations = violations ?? throw new ArgumentNullException(nameof(currentAccount));
            this.currentAccount = currentAccount;
        }

        public override bool IsSatisfiedBy(AccountAllowList candidate)
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
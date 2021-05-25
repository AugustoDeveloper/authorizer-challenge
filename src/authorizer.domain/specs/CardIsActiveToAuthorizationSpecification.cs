using Authorizer.Domain.Entities;
using Authorizer.Domain.Repositories;
using Authorizer.Domain.Specs.Base;

namespace Authorizer.Domain.Specs
{
    public class CardIsActiveToAuthorizationSpecification : CompositeSpecification<Transaction>
    {
        private readonly Account currentAccount;
        private readonly Violations violations;

        public CardIsActiveToAuthorizationSpecification(Account currentAccount, Violations violations)
            => (this.currentAccount, this.violations) = (currentAccount, violations);

        public override bool IsSatisfiedBy(Transaction candidate)
        {
            if (!this.currentAccount.ActiveCard)
            {
                violations.CurrentAccount = currentAccount;
                violations.Add(Violation.CardNotActive);        
                return false;
            }

            return true;
        }
    }
}
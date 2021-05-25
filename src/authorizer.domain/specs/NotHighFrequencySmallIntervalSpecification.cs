using System.Collections.Generic;
using System.Linq;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Specs.Base;

namespace Authorizer.Domain.Specs
{
    public class NotHighFrequencySmallIntervalSpecification: CompositeSpecification<Transaction>
    {
        private readonly IEnumerable<Transaction> lastTwoMinutesTransactions;
        private readonly Violations violations;
        private readonly Account currentAccount;

        public NotHighFrequencySmallIntervalSpecification(IEnumerable<Transaction> lastTwoMinutesTransactions, Account currentAccount, Violations violations)
            => (this.lastTwoMinutesTransactions, this.currentAccount, this.violations) = (lastTwoMinutesTransactions, currentAccount, violations);

        public override bool IsSatisfiedBy(Transaction candidate)
        {
            if (this.lastTwoMinutesTransactions.Count() >= 3)
            {
                violations.CurrentAccount = currentAccount;
                violations.Add(Violation.HighFrequencySmallInterval);
                return false;
            }

            return true;
        }
    }
}
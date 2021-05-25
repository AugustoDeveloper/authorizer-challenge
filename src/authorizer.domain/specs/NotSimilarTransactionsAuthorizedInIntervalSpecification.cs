using System;
using System.Collections.Generic;
using System.Linq;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Specs.Base;

namespace Authorizer.Domain.Specs
{
    public class NotSimilarTransactionsAuthorizedInIntervalSpecification: CompositeSpecification<Transaction>
    {
        private readonly IEnumerable<Transaction> lastTwoMinutesTransactions;
        private readonly Violations violations;
        private readonly Account currentAccount;

        public NotSimilarTransactionsAuthorizedInIntervalSpecification(IEnumerable<Transaction> lastTwoMinutesTransactions, Account currentAccount, Violations violations)
            => (this.lastTwoMinutesTransactions, this.currentAccount, this.violations) = (lastTwoMinutesTransactions, currentAccount, violations);

        public override bool IsSatisfiedBy(Transaction candidate)
        {
            if (lastTwoMinutesTransactions.Any(t => string.Equals(t.Merchant, candidate.Merchant, StringComparison.InvariantCultureIgnoreCase) && t.Amount == candidate.Amount))
            {
                violations.CurrentAccount = currentAccount;
                violations.Add(Violation.DoubleTransaction);
                return false;
            }

            return true;
        }
    }
}
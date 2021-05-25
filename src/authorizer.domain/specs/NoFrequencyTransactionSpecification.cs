using System;
using System.Collections.ObjectModel;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Repositories;
using Authorizer.Domain.Specs.Base;

namespace Authorizer.Domain.Specs
{
    public class NotFrequencyTransactionSpecification : CompositeSpecification<Transaction>
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly Violations violations;
        private readonly Account currentAccount;

        public NotFrequencyTransactionSpecification(ITransactionRepository transactionRepository, Account currentAccount, Violations violations)
            => (this.transactionRepository, this.violations, this.currentAccount) = (transactionRepository, violations, currentAccount);

        public override bool IsSatisfiedBy(Transaction candidate)
        {
            var allLastTwoMinutesTransactions = this.transactionRepository.GetLastTwoMinutesTransactionsFrom(candidate.Time);

            var notHighFrequencySmallIntervalSpec = new NotHighFrequencySmallIntervalSpecification(allLastTwoMinutesTransactions, this.currentAccount, this.violations);
            var notSimilarTrnsactionsAuthorizedSpec = new NotSimilarTransactionsAuthorizedInIntervalSpecification(allLastTwoMinutesTransactions, this.currentAccount, this.violations);

            var notHighFrequencySmallIntervalSpecIsSatisfiedBy = notHighFrequencySmallIntervalSpec.IsSatisfiedBy(candidate);
            var notSimilarTrnsactionsAuthorizedSpecIsSatisfiedBy = notSimilarTrnsactionsAuthorizedSpec.IsSatisfiedBy(candidate);
            var isSatisfiedBy = notHighFrequencySmallIntervalSpecIsSatisfiedBy && notSimilarTrnsactionsAuthorizedSpecIsSatisfiedBy;

            return isSatisfiedBy;
        }
    }
}
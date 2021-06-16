using System;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Repositories;
using Authorizer.Domain.Services.Interfaces;
using Authorizer.Domain.Specs;

namespace Authorizer.Domain.Services
{
    /// <summary>
    /// Represents a way of to execute all logic 
    /// domain of authorization operation
    /// </summary>
    public class AuthorizationService : IAuthorizationService
    {
        private readonly ITransactionRepository transactionRepository;
        private readonly IAccountRepository accountRepository;

        /// <summary>
        /// Create an instance of authorization operation service 
        /// </summary>
        /// <param name="transactionRepository"></param>
        /// <param name="accountRepository"></param>
        public AuthorizationService(ITransactionRepository transactionRepository, IAccountRepository accountRepository)
            => (this.transactionRepository, this.accountRepository) = 
                                (transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository)),
                                accountRepository ?? throw new ArgumentNullException(nameof(accountRepository)));

        /// <inheritdoc cref="IAccountCreationService"/>
        public Violations Authorize(Transaction transaction)
        {
            var violations = new Violations();

            var account = this.accountRepository.GetCurrentAccount();

            var spec = 
                new AccountCreatedToAuthorizationSpecification(account, violations)
                .And(new CardIsActiveToAuthorizationSpecification(account, violations))
                .And(new AccountHasSufficientLimitToAuthorizationSpecification(account, violations));

            if (!account?.AllowListed ?? true)
            {
                spec = spec.And(new NotFrequencyTransactionSpecification(this.transactionRepository, account, violations));
            }
                
            if (spec.IsSatisfiedBy(transaction))
            {
                this.transactionRepository.Add(transaction);
                account.AvailableLimit -= transaction.Amount;
                this.accountRepository.Update(account);
                violations.CurrentAccount = account;
            }

            return violations;
        }
    }
}
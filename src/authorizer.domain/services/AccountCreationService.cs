using System;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Repositories;
using Authorizer.Domain.Services.Interfaces;
using Authorizer.Domain.Specs;

namespace Authorizer.Domain.Services
{
    /// <summary>
    /// Represents a way of to execute all logic 
    /// domain of account creation operation
    /// </summary>
    public class AccountCreationService : IAccountCreationService
    {
        private readonly IAccountRepository accountRepository;

        /// <summary>
        /// Create an instance of account creation operation service 
        /// </summary>
        /// <param name="accountRepository"></param>
        public AccountCreationService(IAccountRepository accountRepository)
            => (this.accountRepository) = (accountRepository ?? throw new ArgumentNullException(nameof(accountRepository)));

        /// <inheritdoc cref="IAccountCreationService"/>
        public Violations CreateAccount(Account account)
        {
            var violations = new Violations();

            var spec = new AccountNotCreatedSpecification(this.accountRepository.GetCurrentAccount(), violations);
            if (spec.IsSatisfiedBy(account))
            {
                this.accountRepository.Add(account);
                violations.CurrentAccount = account;
            }
            
            return violations;
        }
    }
}
using System;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Repositories;
using Authorizer.Domain.Services.Interfaces;
using Authorizer.Domain.Specs;

namespace Authorizer.Domain.Services
{
    public class AccountAllowingListService : IAccountAllowingListService
    {
        private readonly IAccountRepository accountRepository;

        public AccountAllowingListService(IAccountRepository accountRepository)
            => (this.accountRepository) = (accountRepository ?? throw new ArgumentNullException(nameof(accountRepository)));

        public Violations AllowList(AccountAllowList accountAllowList)
        {
            var violations = new Violations();
            var currentAccount = this.accountRepository.GetCurrentAccount();

            //rules
            var spec = new AccountCreatedToAllowListSpecification(currentAccount, violations);
            if (spec.IsSatisfiedBy(accountAllowList))
            {
                currentAccount.AllowListed = accountAllowList.Active == false ? null : true;                
                violations.CurrentAccount = currentAccount;
            }

            return violations;
        }
    }
}
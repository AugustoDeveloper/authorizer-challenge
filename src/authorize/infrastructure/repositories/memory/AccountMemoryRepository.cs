using Authorizer.Domain.Entities;
using Authorizer.Domain.Repositories;

namespace Authorize.Infrastructure.Repositories.Memory
{
    /// <summary>
    /// Account repository to store account information in memory
    /// </summary>
    public class AccountMemoryRepository : IAccountRepository
    {
        /// <summary>
        /// Storing the Account state at memory
        /// </summary>
        /// <value></value>
        public Account CurrentAccount { get; private set; }

        /// <inheritdoc cref="IAccountRepository" />
        public void Add(Account account)
        {
            CurrentAccount = account;
        }

        /// <inheritdoc cref="IAccountRepository" />
        public Account GetCurrentAccount() => CurrentAccount;

        /// <inheritdoc cref="IAccountRepository" />
        public void Update(Account account)
        {
            this.CurrentAccount = account;
        }
    }
}
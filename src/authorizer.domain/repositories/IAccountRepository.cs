using  Authorizer.Domain.Entities;

namespace Authorizer.Domain.Repositories
{
    /// <summary>
    /// Defines a mechanism to store account information
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Get a current account from repository
        /// </summary>
        /// <returns></returns>
        Account GetCurrentAccount();

        /// <summary>
        /// Save an account as current account
        /// </summary>
        /// <param name="account"></param>
        void Add(Account account);

        /// <summary>
        /// Update an account on current account
        /// </summary>
        /// <param name="account"></param>
        void Update(Account account);
    }
}
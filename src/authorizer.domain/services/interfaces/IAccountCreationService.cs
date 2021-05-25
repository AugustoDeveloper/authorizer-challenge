using Authorizer.Domain.Entities;

namespace Authorizer.Domain.Services.Interfaces
{
    /// <summary>
    /// Defines a mechanism to validates an account 
    /// and store this information
    /// </summary>
    public interface IAccountCreationService
    {
        /// <summary>
        /// Create an account validates if has
        /// some violations logic onto call
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Violations CreateAccount(Account account);   
    }
}
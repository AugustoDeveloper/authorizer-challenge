using Authorizer.Domain.Entities;

namespace Authorizer.Domain.Services.Interfaces
{
    /// <summary>
    /// Defines a mechanism to validates an authorization
    /// and store this information
    /// </summary>
    public interface IAuthorizationService 
    {
        /// <summary>
        /// Execute all transaction authorization process
        /// and checking all violations was occurred onto this
        /// execution
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Violations Authorize(Transaction transaction);
    }
}
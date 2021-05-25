using System;
using System.Collections.Generic;
using  Authorizer.Domain.Entities;

namespace Authorizer.Domain.Repositories
{
    /// <summary>
    /// Defines a mechanins to store transaction information
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// Save an transaction into repository
        /// </summary>
        /// <param name="transaction"></param>
        void Add(Transaction transaction);

        /// <summary>
        /// Get all transaction occured at last two minutes 
        /// from the current transaction time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        IEnumerable<Transaction> GetLastTwoMinutesTransactionsFrom(DateTimeOffset time);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Authorizer.Domain.Entities;
using Authorizer.Domain.Repositories;

namespace Authorize.Infrastructure.Repositories.Memory
{
    /// <summary>
    /// Transaction repository to store transation information in memory
    /// </summary>
    public class TransactionMemoryRepository : ITransactionRepository
    {
        /// <summary>
        /// Storing all suceeded transactions
        /// </summary>
        /// <typeparam name="Transaction"></typeparam>
        /// <returns></returns>
        private LinkedList<Transaction> Transactions { get; } = new LinkedList<Transaction>();

        /// <summary>
        /// Storing all succeeded transaction of last two minutes
        /// </summary>
        /// <typeparam name="Transaction"></typeparam>
        /// <returns></returns>
        private List<Transaction> LastTwoMinutesTransactions { get; } = new List<Transaction>();

        /// <summary>
        /// Two minutes interval configuration for query
        /// </summary>
        /// <returns></returns>
        private static TimeSpan TwoMinutesInterval { get; } = TimeSpan.FromMinutes(2);

        /// <inheritdoc cref="ITransactionRepository" />
        public void Add(Transaction transaction)
        {
            // This store transactions as history of 
            // All transactions of the current account
            Transactions.AddLast(transaction);

            // It's store in second data structure but only store last two minutes 
            // transaction, this only is valid if the transactions come in chronological
            // order. For this, all transactions is storing but first thing to do is remove
            // older transaction of two minutes from the transaction time
            LastTwoMinutesTransactions.RemoveAll(t => t.Time < transaction.Time.Subtract(TwoMinutesInterval));
            LastTwoMinutesTransactions.Add(transaction);
        }


        /// <inheritdoc cref="ITransactionRepository" />
        public IEnumerable<Transaction> GetLastTwoMinutesTransactionsFrom(DateTimeOffset time)
        {
            return LastTwoMinutesTransactions.Where(t => t.Time >= time.Subtract(TwoMinutesInterval)).ToList();
        }
    }
}
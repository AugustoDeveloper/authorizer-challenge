using System;

namespace Authorizer.Domain.Entities
{
    /// <summary>
    /// Represents a transaction information
    /// </summary>
    public struct Transaction
    {
        /// <summary>
        /// Merchant of the transaction was executed
        /// </summary>
        /// <value></value>
        public string Merchant { get; }

        /// <summary>
        /// Amount of the transaction 
        /// </summary>
        /// <value></value>
        public uint Amount { get; }

        /// <summary>
        /// Exacly moment at transaction was executed
        /// </summary>
        /// <value></value>
        public DateTimeOffset Time { get; }

        /// <summary>
        /// Create an instance of transaction information
        /// </summary>
        /// <param name="merchant"></param>
        /// <param name="amount"></param>
        /// <param name="time"></param>
        public Transaction(string merchant, uint amount, DateTimeOffset time)
            => (Merchant, Amount, Time) = (merchant, amount, time);
    }
}
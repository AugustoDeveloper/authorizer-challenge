namespace Authorizer.Domain.Entities
{
    /// <summary>
    /// Represent an account information
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Represents the state of activation card
        /// </summary>
        /// <value></value>
        public bool ActiveCard { get; internal set; }

        /// <summary>
        /// Represents the amount available for transaction authorization
        /// </summary>
        /// <value></value>
        public uint AvailableLimit { get; internal set; }

        /// <summary>
        /// Creates an instance of account information
        /// </summary>
        internal Account() {}

        /// <summary>
        /// Creates an instance of account information
        /// </summary>
        /// <param name="activeCard"></param>
        /// <param name="availableLimit"></param>
        public Account(bool activeCard, uint availableLimit)
            => (this.ActiveCard, this.AvailableLimit) = (activeCard, availableLimit);
    }
}
namespace Authorize.Extensions
{
    /// <summary>
    /// Class for mapping to create instance between models and entities
    /// </summary>
    public static class MappingExtensions
    {
        /// <summary>
        /// Transform Account entity instance to Account model
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static Models.Json.Account ToModel(this Authorizer.Domain.Entities.Account account)
            => new Models.Json.Account
            {
                ActiveCard = account?.ActiveCard,
                AvailableLimit = account?.AvailableLimit,
                AllowListed = account?.AllowListed
            };
        
        /// <summary>
        /// Transform Account model instance to Account entity
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static Authorizer.Domain.Entities.Account ToEntity(this Models.Json.Account account)
            => new Authorizer.Domain.Entities.Account(account.ActiveCard ?? false, account.AvailableLimit ?? 0);

        /// <summary>
        /// Transform Transaction entity instance to Transaction model
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static Models.Json.Transaction ToModel(this Authorizer.Domain.Entities.Transaction transaction)
            => new Models.Json.Transaction
            {
                Amount = transaction.Amount,
                Merchant = transaction.Merchant,
                Time = transaction.Time
            };
        
        /// <summary>
        /// Transform Transaction model instance to Transaction entity
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static Authorizer.Domain.Entities.Transaction ToEntity(this Models.Json.Transaction transaction)
            => new Authorizer.Domain.Entities.Transaction(transaction.Merchant, transaction.Amount, transaction.Time);

        public static Authorizer.Domain.Entities.AccountAllowList ToEntity(this Models.Json.AccountAllowList accountAllowList)
            => new Authorizer.Domain.Entities.AccountAllowList(accountAllowList.Active);

    }
}

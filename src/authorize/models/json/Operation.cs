using System.Text.Json.Serialization;

namespace Authorize.Models.Json
{
    /// <summary>
    /// Operation json input model
    /// </summary>
    public class Operation
    {
        /// <summary>
        /// Property store account model
        /// </summary>
        /// <value></value>
        [JsonPropertyName("account")]
        public Account Account { get; set; }

        /// <summary>
        /// Property store transaction model
        /// </summary>
        /// <value></value>
        [JsonPropertyName("transaction")]
        public Transaction Transaction { get; set; }

        [JsonPropertyName("allow-list")]
        public AccountAllowList AllowList { get; set; }

        /// <summary>
        /// Validates if this instance is account operation
        /// </summary>
        /// <returns></returns>
        public OperationKind GetOperationKind()
        { 
            if (Account != null)
                return OperationKind.AccountOperation;
            if (Transaction != null)
                return OperationKind.TransactionOperation;
            
            //FIXME: Not allow this statement
            return OperationKind.AllowListOperation;
        }
    }
}
using System.Text.Json.Serialization;

namespace Authorize.Models.Json
{
    /// <summary>
    /// Operation Result output json model
    /// </summary>
    public class AccountOperationResult
    {
        /// <summary>
        /// Property store the account model
        /// </summary>
        /// <value></value>
        [JsonPropertyName("account")]
        public Account Account { get; set; }

        /// <summary>
        /// Property store the transaction model
        /// </summary>
        /// <value></value>
        [JsonPropertyName("violations")]
        public string[] Violations { get; set; } = new string[0];
    }
}
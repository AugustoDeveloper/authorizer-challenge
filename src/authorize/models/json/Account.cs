using System.Text.Json.Serialization;

namespace Authorize.Models.Json
{
    /// <summary>
    /// Account json model
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Property store if the account has active card
        /// </summary>
        /// <value></value>
        [JsonPropertyName("active-card"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? ActiveCard { get; set; }
        
        /// <summary>
        /// Property store available limit amount
        /// </summary>
        /// <value></value>
        [JsonPropertyName("available-limit"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public uint? AvailableLimit {get;set;}
    }
}
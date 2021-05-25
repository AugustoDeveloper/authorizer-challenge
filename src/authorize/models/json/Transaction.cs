using System;
using System.Text.Json.Serialization;

namespace Authorize.Models.Json
{
    /// <summary>
    /// Transaction json model 
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// Property store merchant information
        /// </summary>
        /// <value></value>
        [JsonPropertyName("merchant")]
        public string Merchant { get; set; }

        /// <summary>
        /// Property store amount transaction information
        /// </summary>
        /// <value></value>
        [JsonPropertyName("amount")]
        public uint Amount { get; set; }

        /// <summary>
        /// Property store transaction time information
        /// </summary>
        /// <value></value>
        [JsonPropertyName("time")]
        public DateTimeOffset Time { get; set; }
    }
}
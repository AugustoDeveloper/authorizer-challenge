using System.Text.Json.Serialization;

namespace Authorize.Models.Json
{
    public class AccountAllowList
    {
        [JsonPropertyName("active")]
        public bool Active { get; set; }
    }
}
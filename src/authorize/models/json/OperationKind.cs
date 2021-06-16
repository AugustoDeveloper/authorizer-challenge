using System.Text.Json.Serialization;

namespace Authorize.Models.Json
{
    public enum OperationKind
    {
        AccountOperation = 0,
        TransactionOperation = 1,
        AllowListOperation = 2
    }
}
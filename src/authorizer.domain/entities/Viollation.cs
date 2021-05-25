using System.ComponentModel;

namespace Authorizer.Domain.Entities
{
    /// <summary>
    /// Represents all types of violation logic may occurr
    /// </summary>
    public enum Violation
    {
        [Description("account-already-initialized")]
        AccountAlreadyInitialized,

        [Description("account-not-initialized")]
        AccountNotInitialized,

        [Description("card-not-active")]
        CardNotActive,

        [Description("insufficient-limit")]
        InsuficientLimit,

        [Description("high-frequency-small-interval")]
        HighFrequencySmallInterval,

        [Description("double-transaction")]
        DoubleTransaction,
    }
}
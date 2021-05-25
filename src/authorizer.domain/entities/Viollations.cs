using System.Collections.Generic;

namespace Authorizer.Domain.Entities
{
    /// <summary>
    /// Represents a operation result with all violations and
    /// current account thats violation belongs
    /// </summary>
    public class Violations : List<Violation> 
    { 
        public Account CurrentAccount { get; internal set; }
    }
}
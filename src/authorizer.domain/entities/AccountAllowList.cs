namespace Authorizer.Domain.Entities
{
    public class AccountAllowList
    {
        public bool Active { get; internal set; }

        internal AccountAllowList() { }

        public AccountAllowList(bool active) { Active = active; }
    }
}
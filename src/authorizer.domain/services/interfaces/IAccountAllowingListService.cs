using Authorizer.Domain.Entities;

namespace Authorizer.Domain.Services.Interfaces
{
    public interface IAccountAllowingListService
    {
        Violations AllowList(AccountAllowList accountAllowList);
    }
}
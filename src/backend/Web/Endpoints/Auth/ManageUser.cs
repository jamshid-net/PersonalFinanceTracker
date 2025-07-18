using FiTrack.Application.Interfaces.Auth;
using FiTrack.Application.Models.Auth;
using FiTrack.Application.QueryFilter;
using FiTrack.Application.Security;
using Shared.Helpers;

namespace Web.Endpoints.Auth;

public class ManageUser : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
      
        group.MapPost(GetAllUsers).RequiredPermission(EnumPermission.GetUser);
    }

    public async Task<ResponseData<PageList<UserResponseModel>>> GetAllUsers(
        ICustomIdentityService service, 
        FilterRequest filterRequest,
        CancellationToken ct)
    => await service.GetAllUsersAsync(filterRequest, ct);
}

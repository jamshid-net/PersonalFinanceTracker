using FiTrack.Application.Interfaces.Auth;
using FiTrack.Application.Models.Auth;
using FiTrack.Application.QueryFilter;
using FiTrack.Application.Security;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;

namespace Web.Endpoints.Auth;

public class Roles : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost(CreateRole).RequiredPermission(EnumPermission.CreateRole);
        group.MapPut(UpdateRole).RequiredPermission(EnumPermission.UpdateRole);
        group.MapGet(GetRoleDetails).RequiredPermission(EnumPermission.GetRole);
        group.MapPost(GetRoles).RequiredPermission(EnumPermission.GetRole);
    }

    public async Task<ResponseData<RoleDetailsResponseModel>> CreateRole(
        IRoleAndPermissionService service,
        [FromBody] RoleRequestModel requestModel,
        CancellationToken ct)
    {
        var result = await service.CreateRoleAsync(requestModel, ct);
        return result;
    }

    public async Task<ResponseData<RoleDetailsResponseModel>> UpdateRole(
        IRoleAndPermissionService service,
        [FromBody] RoleRequestModel requestModel,
        CancellationToken ct)
    {
        var result = await service.UpdateRoleAsync(requestModel, ct);
        return result;
    }


    public async Task<ResponseData<RoleDetailsResponseModel>> GetRoleDetails(
        IRoleAndPermissionService service, 
        int roleId, 
        CancellationToken ct)
    {
        var role = await service.GetRoleByIdAsync(roleId, ct);
        return role;
    }

    public async Task<ResponseData<PageList<RoleResponseModel>>> GetRoles(
        IRoleAndPermissionService service,
        ICurrentUser currentUser, 
        [FromBody] FilterRequest filterRequest, 
        CancellationToken ct)
    {
        var roles = await service.GetAllRolesAsync(filterRequest, ct);
        return roles;
    }

}

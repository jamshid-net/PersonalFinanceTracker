using FiTrack.Application.Models.Auth;
using FiTrack.Application.QueryFilter;

namespace FiTrack.Application.Interfaces.Auth;
public interface IRoleAndPermissionService
{
    Task<RoleDetailsResponseModel> CreateRoleAsync(RoleRequestModel requestModel, CancellationToken ct = default);
    Task<RoleDetailsResponseModel> UpdateRoleAsync(RoleRequestModel requestModel, CancellationToken ct = default);
    Task<RoleDetailsResponseModel> GetRoleByIdAsync(int roleId, CancellationToken ct = default);
    Task<PageList<RoleResponseModel>> GetAllRolesAsync(FilterRequest filterRequest, CancellationToken ct = default);
    Task<int> GetDefaultUserRoleIdAsync(CancellationToken ct = default);
}

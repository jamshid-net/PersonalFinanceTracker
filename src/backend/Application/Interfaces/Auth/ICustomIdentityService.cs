using FiTrack.Application.Models.Auth;
using FiTrack.Application.QueryFilter;
using FiTrack.Domain.Enums;

namespace FiTrack.Application.Interfaces.Auth;
public interface ICustomIdentityService
{
    Task<bool> HasPermissionAsync(int? userId, EnumPermission permission, CancellationToken ct = default);
    Task<bool> HasPermissionAsync(int? userId, IReadOnlyCollection<EnumPermission>? permissions, CancellationToken ct = default);
    Task<string?> GetUserNameAsync(int? userId, CancellationToken ct = default);
    Task<PageList<UserResponseModel>> GetAllUsersAsync(FilterRequest filterRequest, CancellationToken ct = default);
}

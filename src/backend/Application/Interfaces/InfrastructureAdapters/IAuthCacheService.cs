using FiTrack.Domain.Enums;

namespace FiTrack.Application.Interfaces.InfrastructureAdapters;
public interface IAuthCacheService
{
    public Task<List<EnumPermission>> GetPermissionsAsync(int roleId, CancellationToken ct = default);
    public Task<bool> SetPermissionAsync(int roleId, EnumPermission[] permissions, CancellationToken ct = default);
    public Task<bool> RemoveRoleAsync(int roleId, CancellationToken ct = default);
}

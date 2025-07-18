using FiTrack.Application.Interfaces.Auth;
using FiTrack.Application.Interfaces.InfrastructureAdapters;
using FiTrack.Application.Models.Auth;
using FiTrack.Application.QueryFilter;
using FiTrack.Domain.Common;
using FiTrack.Domain.Entities.Auth;
using FiTrack.Domain.Enums;
using FiTrack.Domain.Events;

namespace FiTrack.Application.Services.Auth;
public class CustomIdentityService(IApplicationDbContext dbContext, IMapper mapper) : ICustomIdentityService
{
    public async Task<bool> HasPermissionAsync(int? userId, EnumPermission permission, CancellationToken ct = default)
    {
        if (userId is null)
            return false;

        var hasPermission = await dbContext
                                     .AuthUsers
                                     .Where(u => u.Id == userId)
                                     .AsNoTracking()
                                     .AnyAsync(u => u.Role != null && u.Role.Permissions.Any(p => p.EnumPermission == permission), ct);

        return hasPermission;
    }

    public async Task<bool> HasPermissionAsync(int? userId, IReadOnlyCollection<EnumPermission>? permissions, CancellationToken ct = default)
    {
        if (userId is null || permissions is null || !permissions.Any())
            return false;

        var hasPermission = await dbContext
                                      .AuthUsers
                                      .Where(u => u.Id == userId)
                                      .AsNoTracking()
                                      .AnyAsync(u => u.Role != null && permissions.All(p => u.Role.Permissions.Any(x => x.EnumPermission == p)), ct);
        return hasPermission;
    }

    public async Task<string?> GetUserNameAsync(int? userId, CancellationToken ct = default)
    {
        if (userId is null)
            return null;

        return await dbContext
                                  .AuthUsers
                                  .AsNoTracking()
                                  .Where(u => u.Id == userId).Select(u => u.UserName).FirstOrDefaultAsync(ct);

    }

    public Task<PageList<UserResponseModel>> GetAllUsersAsync(FilterRequest filterRequest, CancellationToken ct = default)
    {
        return dbContext.AuthUsers
            .AsNoTracking()
            .ProjectTo<UserResponseModel>(mapper.ConfigurationProvider)
            .ToPageListAsync(filterRequest, ct);
    }

   
}


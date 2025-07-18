using FiTrack.Application.Interfaces.Auth;
using FiTrack.Application.Interfaces.InfrastructureAdapters;
using FiTrack.Application.Models.Auth;
using FiTrack.Application.QueryFilter;
using FiTrack.Domain.Entities.Auth;

namespace FiTrack.Application.Services.Auth;
public class RoleAndPermissionService(IApplicationDbContext dbContext, 
                                      IMapper mapper, 
                                      IAuthCacheService authCache) : IRoleAndPermissionService
{
    public async Task<RoleDetailsResponseModel> CreateRoleAsync(RoleRequestModel requestModel, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(requestModel.Name))
        {
            throw new ArgumentException("Role name cannot be null or empty.", nameof(requestModel.Name));
        }
        var foundPermissions = await dbContext.AuthPermissions
            .Where(p => requestModel.Permissions.Contains(p.EnumPermission))
            .ToListAsync(ct);

        if (foundPermissions.Count != requestModel.Permissions.Length)
        {
            throw new InvalidOperationException("One or more permission IDs are invalid.");
        }

        var role = new AuthRole
        {
            Name = requestModel.Name,
            Permissions = foundPermissions
        };

        var entityEntry = dbContext.AuthRoles.Add(role);
        var isSaved = await dbContext.SaveChangesAsync(ct) > 0;

        if (isSaved)
        {
            await authCache.SetPermissionAsync(role.Id, requestModel.Permissions, ct);
        }
        return mapper.Map<RoleDetailsResponseModel>(entityEntry.Entity);
    }

    public async Task<PageList<RoleResponseModel>> GetAllRolesAsync(FilterRequest filterRequest, CancellationToken ct = default)
    {
        var result = await dbContext.AuthRoles
            .AsNoTracking()
            .ProjectTo<RoleResponseModel>(mapper.ConfigurationProvider)
            .ToPageListAsync(filterRequest, ct);


        return result;
    }

    public async Task<int> GetDefaultUserRoleIdAsync(CancellationToken ct = default)
    {
        var  defaultRoleId = await dbContext.AuthRoles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Name == "User", ct);

        if (defaultRoleId is null)
        {
            var newRole = new AuthRole
            {
                Name = "User",
                Permissions = []
            };
            var entityEntry = dbContext.AuthRoles.Add(newRole);
            await dbContext.SaveChangesAsync(ct);

            await authCache.SetPermissionAsync(entityEntry.Entity.Id, [], ct);

            return entityEntry.Entity.Id;
        }

        return defaultRoleId.Id;
    }

    public async Task<RoleDetailsResponseModel> GetRoleByIdAsync(int roleId, CancellationToken ct = default)
    {
        var roleDetails = await dbContext.AuthRoles.AsNoTracking()
            .ProjectTo<RoleDetailsResponseModel>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(x => x.Id == roleId, ct);

        if(roleDetails is null)
        {
            throw new NotFoundException(roleId.ToString(), nameof(RoleDetailsResponseModel));
        }

        return roleDetails;
    }

    public async Task<RoleDetailsResponseModel> UpdateRoleAsync(RoleRequestModel requestModel, CancellationToken ct = default)
    {
        var role = await dbContext.AuthRoles.Include(x => x.Permissions).FirstOrDefaultAsync(x => x.Id == requestModel.Id, ct);
        if (role == null)
        {
            throw new NotFoundException(requestModel.Id.ToString(), nameof(AuthRole));
        }
        role.Name = requestModel.Name;
        role.Permissions = await dbContext.AuthPermissions
            .Where(p => requestModel.Permissions.Contains(p.EnumPermission))
            .ToListAsync(ct);

        var entityEntry = dbContext.AuthRoles.Update(role);
        var isSaved = await dbContext.SaveChangesAsync(ct) > 0;

        if (isSaved)
        {
            await authCache.SetPermissionAsync(role.Id, requestModel.Permissions, ct);
        }
        return mapper.Map<RoleDetailsResponseModel>(entityEntry.Entity);
    }
}

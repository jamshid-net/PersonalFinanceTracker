using FiTrack.Application.Interfaces.InfrastructureAdapters;
using FiTrack.Domain.Common;
using FiTrack.Domain.Entities.Auth;
using FiTrack.Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FiTrack.Infrastructure.Data;

public static class InitializerExtensions
{
    public static void AddAsyncSeeding(this DbContextOptionsBuilder builder, IServiceProvider serviceProvider)
    {
        builder.UseAsyncSeeding(async (context, _, ct) =>
        {
            var initializer = serviceProvider.GetRequiredService<ApplicationDbContextInitializer>();

            await initializer.SeedAsync();
        });
    }

    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();

        await initializer.InitialiseAsync();
    }
}

public class ApplicationDbContextInitializer(
    ILogger<ApplicationDbContextInitializer> logger,
    IAuthCacheService cacheService,
    ApplicationDbContext context)
{
    public async Task InitialiseAsync()
    {
        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedPermissionsAsync();
            await TrySeedUserAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
        finally
        {
            await LoadPermissionToCacheAsync();
        }
    }
    public async Task TrySeedUserAsync()
    {
        var user = await context.AuthUsers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.UserName == "admin");

        if (user == null)
        {
            var hashSalt = CryptoPassword.CreateHashSalted("admin@123");
            var roleId = await TrySeedRoleAsync();
            user = new AuthUser
            {
                UserName = "admin",
                FirstName = "Jamshid",
                LastName = "Ashurov",
                RoleId = roleId,
            };
            user.SetPassword(hashSalt);
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();
        }
    }
    public async Task TrySeedPermissionsAsync()
    {
        var defaultPermissions = DefaultPermissions();
        var existingPermissions = await context.AuthPermissions
            .IgnoreQueryFilters()
            .Where(p => defaultPermissions.Select(dp => dp.Id).Contains(p.Id))
            .ToListAsync();

        foreach (var permission in defaultPermissions)
        {
            var existingPermission = existingPermissions.FirstOrDefault(p => p.Id == permission.Id);

            if (existingPermission == null)
            {
                // Insert the new permission without the need for IDENTITY_INSERT
                context.AuthPermissions.Add(permission);
                await context.SaveChangesAsync();
            }
            else
            {
                // Update existing permission if necessary
                if (existingPermission.Name != permission.Name || existingPermission.EnumPermission != permission.EnumPermission)
                {
                    existingPermission.EnumPermission = permission.EnumPermission;
                    context.AuthPermissions.Update(existingPermission);
                    await context.SaveChangesAsync();
                }
            }
        }

        var defaultPermissionIds = defaultPermissions.Select(dp => dp.Id);
        await context.AuthPermissions.Where(p => !defaultPermissionIds.Contains(p.Id))
            .ExecuteDeleteAsync();
    }
    private async Task<int> TrySeedRoleAsync()
    {
        var role = await context.AuthRoles
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Name == "Administrator");

        var permissions = await context.AuthPermissions
            .IgnoreQueryFilters()
            .ToListAsync();

        if (role is null)
        { 
            var newRole = new AuthRole
            {
                Name = "Administrator",
                Permissions = permissions,
                        
            };
            context.AuthRoles.Add(newRole);

            await context.SaveChangesAsync();
            return newRole.Id;
        }
        var userRole =  await context.AuthRoles.IgnoreQueryFilters().Where(r => r.Name == "User").FirstOrDefaultAsync();
        if (userRole is null)
        {
            userRole = new AuthRole
            {
                Name = "User",
                Permissions = []
            };
            context.AuthRoles.Add(userRole);
            await context.SaveChangesAsync();
        }
        else
        {
            userRole.Permissions = [];
            context.AuthRoles.Update(userRole);
            await context.SaveChangesAsync();
        }

        return role.Id;
    }
    private async Task LoadPermissionToCacheAsync()
    {
        var roleIdAndPermissions = await context.AuthRoles.AsNoTracking()
                     .Select(x => new
                     {
                         RoleId = x.Id,
                         EnumPermissions = x.Permissions.Select(p => p.EnumPermission)
                     }).ToDictionaryAsync(x => x.RoleId, y => y.EnumPermissions.ToArray());


        foreach (var roleIdAndPermission in roleIdAndPermissions)
        {
            await cacheService.SetPermissionAsync(roleIdAndPermission.Key, roleIdAndPermission.Value);
        }
    }

    private static AuthPermission[] DefaultPermissions()
    {
        var enumPermissions = Enum.GetValues<EnumPermission>();

        var authPermissions = enumPermissions.Select(permission =>
        {
            if (permission == 0)
            {
                throw new InvalidOperationException($"{permission.ToString()} cannot be start value from 0. Permission ID cannot be 0.");
            }
            return new AuthPermission
            {
                Id = (int)permission,
                EnumPermission = permission
            };
        }).ToArray();
        return authPermissions;
    }
}

using FiTrack.Application.Interfaces.InfrastructureAdapters;
using FiTrack.Domain.Enums;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shared.Constants;

namespace FiTrack.Application.Security;
public static class MinimalApiAuthExtensions
{
    public static RouteHandlerBuilder RequiredPermission(
        this RouteHandlerBuilder builder,
        params EnumPermission[] enumPermissions)
    {
        builder.AddEndpointFilter(async (context, next) =>
        {
            var httpContext = context.HttpContext;
            var user = httpContext.User;

            // 1. Authenticated?
            if (user?.Identity is { IsAuthenticated: false })
                return Results.Unauthorized();
            
            // 2. Permissions check
            if (enumPermissions is { Length: > 0 })
            {
                if (!int.TryParse(user?.FindFirst(StaticClaims.RoleId)?.Value, out int userRoleId))
                    return Results.Forbid();
                var cacheService = httpContext.RequestServices.GetRequiredService<IAuthCacheService>();

                var permissions = await cacheService.GetPermissionsAsync(userRoleId);
                if (!permissions.Intersect(enumPermissions).Any())
                    return Results.Forbid();
            }

            return await next(context);
        });

        return builder;
    }

    //private static async Task<>
}


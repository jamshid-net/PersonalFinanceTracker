using FiTrack.Application.Interfaces.Auth;
using FiTrack.Application.Interfaces.InfrastructureAdapters;
using FiTrack.Application.Models.Auth;
using FiTrack.Domain.Common;
using FiTrack.Domain.Entities.Auth;
using FiTrack.Domain.Events;

namespace FiTrack.Application.Services.Auth;
public class AuthUserService(ITokenService tokenService,
                             IApplicationDbContext dbContext,
                             IRoleAndPermissionService roleAndPermissionService) : IAuthUserService
{
    public Task<TokenResponseModel> LoginAsync(LoginRequestModel loginRequest, CancellationToken ct = default)
    => tokenService.GenerateTokenAsync(loginRequest, ct);


    public  Task<bool> LogoutAsync(string refreshToken, CancellationToken ct = default)
    => tokenService.DeleteRefreshTokenAsync(refreshToken, ct);
    
    
    public Task<TokenResponseModel> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    => tokenService.RefreshTokenAsync(refreshToken, ct);

    public async Task<TokenResponseModel> RegisterAsync(RegisterUserRequestModel registerUserRequestModel, CancellationToken ct = default)
    {
        if(registerUserRequestModel.UserName.Contains(" "))
            throw new ArgumentException("Username cannot contain spaces.", nameof(registerUserRequestModel.UserName));

        var existingUser = await dbContext.AuthUsers
            .AnyAsync(u => u.UserName == registerUserRequestModel.UserName, ct);

        if (existingUser)
            throw new InvalidOperationException($"User with username '{registerUserRequestModel.UserName}' already exists.");

        var hashSalt = CryptoPassword.CreateHashSalted(registerUserRequestModel.Password);

        var roleId = await roleAndPermissionService.GetDefaultUserRoleIdAsync(ct);

        var user = new AuthUser
        {
            FirstName = registerUserRequestModel.FirstName,
            LastName = registerUserRequestModel.LastName,
            UserName = registerUserRequestModel.UserName,
            IsActive = true,
            LastLogin = DateTimeOffset.UtcNow,
            RoleId = roleId
            
        };
        user.SetPassword(hashSalt);
        user.AddDomainEvent(new AuthUserCreatedEvent(user));
        var entryEntity =  await dbContext.AuthUsers.AddAsync(user, ct);
        await dbContext.SaveChangesAsync(ct);

        return await tokenService.GenerateTokenAsync(new LoginRequestModel(entryEntity.Entity.UserName, registerUserRequestModel.Password, string.Empty), ct);
    }
}

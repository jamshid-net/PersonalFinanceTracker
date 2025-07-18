using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FiTrack.Application.Interfaces.Auth;
using FiTrack.Application.Interfaces.InfrastructureAdapters;
using FiTrack.Application.Models.Auth;
using FiTrack.Domain.Common;
using FiTrack.Domain.Entities.Auth;
using FiTrack.Domain.Exceptions;
using Microsoft.IdentityModel.Tokens;
using Shared.Constants;
using Shared.Extensions;

namespace FiTrack.Application.Services.Auth;
public class TokenService(IApplicationDbContext dbContext) : ITokenService
{
    public async Task<TokenResponseModel> GenerateTokenAsync(LoginRequestModel loginRequest, CancellationToken ct = default)
    {
        if (loginRequest is null)
        {
            throw new ArgumentNullException(nameof(loginRequest), "Login command cannot be null.");
        }
        var foundUser = await dbContext.AuthUsers.SingleOrDefaultAsync(x => x.UserName == loginRequest.UserName, ct);

        if (foundUser is null)
            throw new NotFoundException(loginRequest.UserName, nameof(AuthUser));

        var comingHashedPassword = CryptoPassword.GetHashSalted(loginRequest.Password, foundUser.PasswordSalt);

        if (foundUser.PasswordHash != comingHashedPassword)
        {
            foundUser.FailedLoginAttempts++;
            dbContext.AuthUsers.Update(foundUser);
            await dbContext.SaveChangesAsync(ct);
            throw new ErrorFromClientException("Invalid username or password.");
        }
        else
        {
            foundUser.LastLogin = DateTimeOffset.UtcNow;
            dbContext.AuthUsers.Update(foundUser);
        }

        var refreshToken = Guid.NewGuid().ToLowerString();

        var userToken = new AuthUserRefreshToken
        {
            UserId = foundUser.Id,
            DeviceId = loginRequest.DeviceId,
            RefreshToken = refreshToken,
            UpdateDate = DateTimeOffset.UtcNow
        };
        await InsertToken(userToken, ct);

        return GetJwt(userToken, refreshToken);

    }

    public async Task<TokenResponseModel> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(refreshToken))
            throw new ArgumentNullException(nameof(refreshToken), "Refresh token cannot be null or empty.");

        var userToken = await FindTokenAsync(refreshToken, ct);

        if (userToken is null)
            throw new NotFoundException(refreshToken, nameof(AuthUserRefreshToken));


        var min = DateTimeOffset.Now.Subtract(userToken.UpdateDate).TotalMinutes;

        if (min > AuthOptions.ExpireMinutesRefresh)
            throw new RefreshTokenExpiredException("Refresh token has expired");


        var newRefreshToken = Guid.NewGuid().ToLowerString();

        //expire the old refresh_token and add a new refresh_token
        userToken.RefreshToken = newRefreshToken;
        userToken.UpdateDate = DateTimeOffset.UtcNow;


        var data = GetJwt(userToken, newRefreshToken);
        dbContext.AuthUserRefreshTokens.Update(userToken);
        await dbContext.SaveChangesAsync(ct);
        return data;
    }

    public async Task<bool> DeleteRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new ArgumentNullException(nameof(refreshToken), "Refresh token cannot be null or empty.");
        }
        return await dbContext.AuthUserRefreshTokens
                        .Where(x => x.RefreshToken == refreshToken)
                        .ExecuteDeleteAsync(cancellationToken) > 0;
    }


    private TokenResponseModel GetJwt(AuthUserRefreshToken authUserToken, string refreshToken)
    {
        var utcNow = DateTime.UtcNow;
        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToLowerString()),
            new (JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(CultureInfo.InvariantCulture)),
            new (StaticClaims.DeviceId, authUserToken.DeviceId ?? string.Empty),
            new (StaticClaims.UserId, authUserToken.UserId.ToString()),
            new (StaticClaims.RoleId, authUserToken.User?.RoleId.ToString() ?? "0"),
        ];


        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.Issuer,
            audience: AuthOptions.Audience,
            claims: claims,
            notBefore: utcNow,
            expires: utcNow.Add(TimeSpan.FromMinutes(AuthOptions.ExpireMinutes)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256)
        );

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return new TokenResponseModel(encodedJwt,
                                                refreshToken,
                            utcNow.Add(TimeSpan.FromMinutes(AuthOptions.ExpireMinutes)),
                            utcNow.Add(TimeSpan.FromMinutes(AuthOptions.ExpireMinutesRefresh)));
    }

    private async Task<AuthUserRefreshToken> InsertToken(AuthUserRefreshToken authUserToken, CancellationToken ct = default)
    {
        var userTokens = await dbContext.AuthUserRefreshTokens.Where(x => x.UserId == authUserToken.UserId)
                                                           .ToListAsync(ct);

        var idsToKeep = userTokens
            .OrderByDescending(x => x.Id)
            .Take(AuthOptions.MaxDeviceCount)
            .Select(x => x.Id)
            .ToList();

        await dbContext.AuthUserRefreshTokens
            .Where(x => x.UserId == authUserToken.UserId && !idsToKeep.Contains(x.Id))
            .ExecuteDeleteAsync(ct);

        await dbContext.AuthUserRefreshTokens.AddAsync(authUserToken, ct);

        await dbContext.SaveChangesAsync(ct);

        return authUserToken;
    }

    private Task<AuthUserRefreshToken?> FindTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        return dbContext.AuthUserRefreshTokens
                         .Include(x => x.User)
                         .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken, ct);
    }
}

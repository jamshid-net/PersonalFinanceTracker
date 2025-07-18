using FiTrack.Application.Models.Auth;

namespace FiTrack.Application.Interfaces.Auth;
public interface ITokenService
{
    Task<TokenResponseModel> GenerateTokenAsync(LoginRequestModel loginRequest, CancellationToken ct = default);
    Task<TokenResponseModel> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<bool> DeleteRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
}

using FiTrack.Application.Models.Auth;

namespace FiTrack.Application.Interfaces.Auth;
public interface IAuthUserService
{
    Task<TokenResponseModel> LoginAsync(LoginRequestModel loginRequest, CancellationToken ct = default);
    Task<bool> LogoutAsync(string refreshToken, CancellationToken ct = default);
    Task<TokenResponseModel> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<TokenResponseModel> RegisterAsync(RegisterUserRequestModel registerUserRequestModel, CancellationToken ct = default);

}

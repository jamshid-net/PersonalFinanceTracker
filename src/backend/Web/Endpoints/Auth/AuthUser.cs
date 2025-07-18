using FiTrack.Application.Interfaces.Auth;
using FiTrack.Application.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;

namespace Web.Endpoints.Auth;

public class AuthUser : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost(Login);
        group.MapPut(RefreshToken);
        group.MapDelete(Logout);
        group.MapPost(Register);

    }

    public async Task<ResponseData<TokenResponseModel>> Login(
        IAuthUserService service, 
        [FromBody] LoginRequestModel requestModel,
        CancellationToken ct)
    => await service.LoginAsync(requestModel, ct);


    public async Task<ResponseData<TokenResponseModel>> Register(
        IAuthUserService service, 
        [FromBody] RegisterUserRequestModel requestModel,
        CancellationToken ct)
        => await service.RegisterAsync(requestModel, ct);

    public async Task<ResponseData<TokenResponseModel>> RefreshToken(
        IAuthUserService service, 
        [FromQuery] string refreshToken,
        CancellationToken ct)
        => await service.RefreshTokenAsync(refreshToken, ct);

    public async Task<ResponseData<bool>> Logout(
        IAuthUserService service,
        [FromQuery] string refreshToken,
        CancellationToken ct)
    => await service.LogoutAsync(refreshToken, ct);

}

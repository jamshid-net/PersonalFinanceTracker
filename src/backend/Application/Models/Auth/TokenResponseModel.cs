namespace FiTrack.Application.Models.Auth;
public record TokenResponseModel(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiration,
    DateTimeOffset RefreshTokenExpiration
);

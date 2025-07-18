namespace FiTrack.Application.Models.Auth;
public record LoginRequestModel(string UserName, string Password, string? DeviceId);

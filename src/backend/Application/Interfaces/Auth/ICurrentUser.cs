namespace FiTrack.Application.Interfaces.Auth;

public interface ICurrentUser
{
    int? Id { get; }
    int? RoleId { get; }
}

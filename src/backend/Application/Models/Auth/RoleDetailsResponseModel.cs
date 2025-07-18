using FiTrack.Domain.Enums;

namespace FiTrack.Application.Models.Auth;
public class RoleDetailsResponseModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<string> Permissions { get; set; } = [];
}

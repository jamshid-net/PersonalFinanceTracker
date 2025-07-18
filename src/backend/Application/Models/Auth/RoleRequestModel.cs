using FiTrack.Domain.Enums;

namespace FiTrack.Application.Models.Auth;
public record RoleRequestModel(int Id, string Name, EnumPermission[] Permissions);


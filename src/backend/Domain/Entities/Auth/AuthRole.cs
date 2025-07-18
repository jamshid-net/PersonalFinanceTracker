using FiTrack.Domain.Common;

namespace FiTrack.Domain.Entities.Auth;
public class AuthRole : BaseAuditableEntity
{
    public required string Name { get; set; }
    public virtual ICollection<AuthPermission> Permissions { get; set; } = [];
}

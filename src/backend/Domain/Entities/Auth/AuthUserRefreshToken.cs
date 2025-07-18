using FiTrack.Domain.Common;

namespace FiTrack.Domain.Entities.Auth;
public class AuthUserRefreshToken : BaseEntity
{
    public required string RefreshToken { get; set; }
    public int UserId { get; set; }
    public virtual AuthUser? User { get; set; }
    public string? DeviceId { get; set; }
    public DateTimeOffset UpdateDate { get; set; }
}

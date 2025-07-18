using FiTrack.Domain.Common;

namespace FiTrack.Domain.Entities.Auth;
public class AuthUser : BaseAuditableEntity
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public DateTimeOffset LastLogin { get; set; }
    public string PasswordHash { get; private set; } = null!;
    public string PasswordSalt { get; private set; } = null!;
    public int FailedLoginAttempts { get; set; } = 0;
    public int RoleId { get; set; }
    public virtual AuthRole? Role { get; set; }
    public void SetPassword(HashSalt hashSalt)
    {
        PasswordHash = hashSalt.Hash;
        PasswordSalt = hashSalt.Salt;
    }
}

namespace FiTrack.Application.Models.Auth;
public class UserResponseModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public bool IsActive { get; set; } = true;
    public DateTimeOffset LastLogin { get; set; }
    public int FailedLoginAttempts { get; set; }
    public int RoleId { get; set; }
}

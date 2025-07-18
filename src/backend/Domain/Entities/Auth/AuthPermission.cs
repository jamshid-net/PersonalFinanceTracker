using FiTrack.Domain.Common;
using FiTrack.Domain.Enums;

namespace FiTrack.Domain.Entities.Auth;
public class AuthPermission : BaseEntity
{
    public string Name { get; private set; } = null!;
    public required EnumPermission EnumPermission
    {
        get => _enumPermission;
        set
        {
            _enumPermission = value;
            Name = value.ToString(); 
        }
    }
    private EnumPermission _enumPermission;
    public virtual ICollection<AuthRole> Roles { get; set; } = [];
}

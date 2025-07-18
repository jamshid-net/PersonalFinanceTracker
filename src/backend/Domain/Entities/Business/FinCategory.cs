using FiTrack.Domain.Common;
using FiTrack.Domain.Entities.Auth;
using FiTrack.Domain.ValueObjects;

namespace FiTrack.Domain.Entities.Business;
public class FinCategory : BaseAuditableEntity
{
    public string Name { get; set; } = null!;
    public Colour Colour { get; set; } = Colour.White;

    public int UserId { get; set; }
    public virtual AuthUser? User { get; set; }
    public virtual ICollection<FinTransaction> Transactions { get; set; } = [];
}

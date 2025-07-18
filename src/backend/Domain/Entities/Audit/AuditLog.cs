using FiTrack.Domain.Common;

namespace FiTrack.Domain.Entities.Audit;
public class AuditLog : BaseAuditableEntity
{
    public int? UserId { get; set; }
    public required string Action { get; set; }
    public required string EntityName { get; set; }
    public int EntityId { get; set; }

    public string? OldValue { get; set; } 
    public string? NewValue { get; set; }
}

using System.ComponentModel.DataAnnotations;
using FiTrack.Domain.Common;
using FiTrack.Domain.Entities.Auth;
using FiTrack.Domain.Enums;

namespace FiTrack.Domain.Entities.Business;
public class FinTransaction : BaseAuditableEntity
{
    public EnumTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }

    public int CategoryId { get; set; }
    public virtual FinCategory? Category { get; set; }

    public int UserId { get; set; }
    public virtual AuthUser? User { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = default!;

}

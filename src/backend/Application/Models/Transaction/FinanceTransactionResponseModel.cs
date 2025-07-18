using FiTrack.Application.Models.Base;
using FiTrack.Application.Models.Category;
using FiTrack.Domain.Enums;

namespace FiTrack.Application.Models.Transaction;
public class FinanceTransactionResponseModel : BaseAuditResponseModel
{
    public int Id { get; set; }
    public EnumTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public int CategoryId { get; set; }
    public CategoryResponseModel? Category { get; set; }
    public int UserId { get; set; }
}

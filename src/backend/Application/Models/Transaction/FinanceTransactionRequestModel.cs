using FiTrack.Domain.Enums;

namespace FiTrack.Application.Models.Transaction;
public class FinanceTransactionRequestModel
{
    public int? Id { get; set; }
    public EnumTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string? Note { get; set; }
    public int CategoryId { get; set; }
}

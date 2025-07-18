using FiTrack.Domain.Enums;

namespace FiTrack.Application.Models.Transaction;
public class FinanceMiniTransactionResponseModel
{
    public int Id { get; set; }
    public EnumTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public string CategoryName { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public DateTimeOffset LastModified { get; set; }
}

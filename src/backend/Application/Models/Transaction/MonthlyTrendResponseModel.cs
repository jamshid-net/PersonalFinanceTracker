namespace FiTrack.Application.Models.Transaction;
public class MonthlyTrendResponseModel
{
    public string Month { get; set; } = default!;
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance => TotalIncome - TotalExpense;
}

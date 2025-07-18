namespace FiTrack.Application.Models.Transaction;
public class MonthlySummaryResponseModel
{
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance => TotalIncome - TotalExpense;
}

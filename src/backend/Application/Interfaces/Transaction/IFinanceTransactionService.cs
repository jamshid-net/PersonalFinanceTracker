using FiTrack.Application.Models.Transaction;
using FiTrack.Application.QueryFilter;

namespace FiTrack.Application.Interfaces.Transaction;
public interface IFinanceTransactionService
{
    Task<FinanceTransactionResponseModel> CreateAsync(
        FinanceTransactionRequestModel requestModel,
        CancellationToken cancellationToken = default);

    Task<FinanceTransactionResponseModel> UpdateAsync(
        FinanceTransactionRequestModel requestModel,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, 
        CancellationToken cancellationToken = default);

    Task<FinanceTransactionResponseModel> GetByIdAsync(int id, 
        CancellationToken cancellationToken = default);

    Task<PageList<FinanceMiniTransactionResponseModel>> GetAllAsync(
        FilterRequest filterRequest, 
        CancellationToken cancellationToken = default);

    Task<MonthlySummaryResponseModel> GetCurrentMonthSummaryAsync(CancellationToken cancellationToken = default);
    Task<List<MonthlyTrendResponseModel>> GetMonthlyTrendsAsync(int months = 6, CancellationToken ct = default);

}

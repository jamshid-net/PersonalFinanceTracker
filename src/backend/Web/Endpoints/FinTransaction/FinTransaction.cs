using FiTrack.Application.Interfaces.Transaction;
using FiTrack.Application.Models.Transaction;
using FiTrack.Application.QueryFilter;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;

namespace Web.Endpoints.FinTransaction;

public class FinTransaction : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost(CreateTransaction).RequireAuthorization();
        group.MapPut(UpdateTransaction).RequireAuthorization();
        group.MapDelete(DeleteTransaction).RequireAuthorization();
        group.MapGet(GetTransactionById).RequireAuthorization();
        group.MapPost(GetTransactions).RequireAuthorization();

        group.MapGet(GetCurrentMonthSummary).RequireAuthorization();
        group.MapGet(GetMonthlyTrends).RequireAuthorization();
    }

    public async Task<ResponseData<FinanceTransactionResponseModel>> CreateTransaction(
        IFinanceTransactionService service,
        [FromBody] FinanceTransactionRequestModel requestModel,
        CancellationToken ct)
    {
        return await service.CreateAsync(requestModel, ct);
    }

    public async Task<ResponseData<FinanceTransactionResponseModel>> UpdateTransaction(
        IFinanceTransactionService service,
        [FromBody] FinanceTransactionRequestModel requestModel,
        CancellationToken ct)
    {
        return await service.UpdateAsync(requestModel, ct);
    }

    public async Task<ResponseData<bool>> DeleteTransaction(
        IFinanceTransactionService service,
        [AsParameters] int id,
        CancellationToken ct)
    {
        return await service.DeleteAsync(id, ct);
    }

    public async Task<FinanceTransactionResponseModel> GetTransactionById(
        IFinanceTransactionService service,
        [AsParameters] int id,
        CancellationToken ct)
    {
        return await service.GetByIdAsync(id, ct);
    }

    public async Task<ResponseData<PageList<FinanceMiniTransactionResponseModel>>> GetTransactions(
        IFinanceTransactionService service,
        [FromBody] FilterRequest request,
        CancellationToken ct)
    {
        return await service.GetAllAsync(request, ct);
    }


    public async Task<ResponseData<MonthlySummaryResponseModel>> GetCurrentMonthSummary(
        IFinanceTransactionService service,
        CancellationToken ct)
    {
        return await service.GetCurrentMonthSummaryAsync(ct);
    }

    public async Task<ResponseData<List<MonthlyTrendResponseModel>>> GetMonthlyTrends(
        IFinanceTransactionService service,
        [AsParameters] int months,
        CancellationToken ct)
    {
        return await service.GetMonthlyTrendsAsync(months, ct);
    }
}

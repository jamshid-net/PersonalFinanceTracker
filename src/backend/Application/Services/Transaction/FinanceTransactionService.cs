using FiTrack.Application.Interfaces.Auth;
using FiTrack.Application.Interfaces.InfrastructureAdapters;
using FiTrack.Application.Interfaces.Transaction;
using FiTrack.Application.Models.Transaction;
using FiTrack.Application.QueryFilter;
using FiTrack.Domain.Entities.Business;
using FiTrack.Domain.Enums;

namespace FiTrack.Application.Services.Transaction;
public class FinanceTransactionService(ICurrentUser currentUser,
                                       IApplicationDbContext dbContext,
                                       TimeProvider dateTime,
                                       IMapper mapper) : IFinanceTransactionService
{
    public async Task<FinanceTransactionResponseModel> CreateAsync(FinanceTransactionRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var userId = currentUser.Id;
        if (userId is null or 0)
            throw new UnauthorizedAccessException("User is not authenticated");

        var category = await dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == requestModel.CategoryId && c.UserId == userId, cancellationToken);

        if (category is null)
            throw new NotFoundException(requestModel.CategoryId.ToString(), nameof(Category));

        var transaction = new FinTransaction
        {
            Amount = requestModel.Amount,
            Type = requestModel.Type,
            Note = requestModel.Note,
            CategoryId = requestModel.CategoryId,
            UserId = (int)userId,

        };

        var entityEntry = dbContext.FinTransactions.Add(transaction);
        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<FinanceTransactionResponseModel>(entityEntry.Entity);
    }

    public async Task<FinanceTransactionResponseModel> UpdateAsync(FinanceTransactionRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var userId = currentUser.Id;
        if (userId is null or 0)
            throw new UnauthorizedAccessException("User is not authenticated");

        var category = await dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == requestModel.CategoryId && c.UserId == userId, cancellationToken);

        if (category is null)
            throw new NotFoundException(requestModel.CategoryId.ToString(), nameof(Category));
        var foundTransaction = await dbContext.FinTransactions
                                                               .FirstOrDefaultAsync(t => t.Id == requestModel.Id && t.UserId == userId, cancellationToken);

        if (foundTransaction is null)
            throw new NotFoundException(requestModel.Id.ToString() ?? "", nameof(FinTransaction));


        foundTransaction.Amount = requestModel.Amount;
        foundTransaction.Type = requestModel.Type;
        foundTransaction.Note = requestModel.Note;
        foundTransaction.CategoryId = requestModel.CategoryId;


        var entityEntry = dbContext.FinTransactions.Update(foundTransaction);
        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<FinanceTransactionResponseModel>(entityEntry.Entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var userId = currentUser.Id;
        if (userId == 0)
            throw new UnauthorizedAccessException("User is not authenticated");

        var transaction = await dbContext.FinTransactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId && t.IsActive, cancellationToken);

        if (transaction is null)
            throw new NotFoundException(id.ToString(), nameof(FinTransaction));

        transaction.IsActive = false;

        return await dbContext.SaveChangesAsync(cancellationToken) > 0;

    }

    public async Task<FinanceTransactionResponseModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var userId = currentUser.Id;
        if (userId is null or 0)
            throw new UnauthorizedAccessException("User is not authenticated");

        var foundTransaction = await dbContext.FinTransactions
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId && t.IsActive, cancellationToken);

        if (foundTransaction is null)
            throw new NotFoundException(id.ToString(), nameof(FinTransaction));

        return mapper.Map<FinanceTransactionResponseModel>(foundTransaction);
    }

    public Task<PageList<FinanceMiniTransactionResponseModel>> GetAllAsync(FilterRequest filterRequest, CancellationToken cancellationToken = default)
    {
        var userId = currentUser.Id;
        if (userId is null or 0)
            throw new UnauthorizedAccessException("User is not authenticated");

        var query = dbContext.FinTransactions.Where(t => t.UserId == userId);

        return query.ProjectTo<FinanceMiniTransactionResponseModel>(mapper.ConfigurationProvider).ToPageListAsync(filterRequest, cancellationToken);

    }

    public async Task<MonthlySummaryResponseModel> GetCurrentMonthSummaryAsync(CancellationToken cancellationToken = default)
    {
        var userId = currentUser.Id;
        if (userId == 0)
            throw new UnauthorizedAccessException();

        var now = dateTime.GetUtcNow();
        var firstDay = new DateTimeOffset(new DateTime(now.Year, now.Month, 1), TimeSpan.Zero);

        var query = dbContext.FinTransactions
            .Where(t => t.UserId == userId &&
                        t.Created >= firstDay &&
                        t.IsActive);

        var income = await query.Where(x => x.Type == EnumTransactionType.Income)
            .SumAsync(x => x.Amount, cancellationToken);

        var expense = await query.Where(x => x.Type == EnumTransactionType.Expense)
                         .SumAsync(x => x.Amount, cancellationToken);

        return new MonthlySummaryResponseModel
        {
            TotalIncome = income,
            TotalExpense = expense
        };
    }

    public async Task<List<MonthlyTrendResponseModel>> GetMonthlyTrendsAsync(int months = 6, CancellationToken ct = default)
    {
        var userId = currentUser.Id;
        var fromDate = DateTime.UtcNow.AddMonths(-months);


        var result = await dbContext.FinTransactions
            .Where(t => t.UserId == userId && t.Created >= fromDate && t.IsActive)
            .GroupBy(t => new { t.Created.Year, t.Created.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                TotalIncome = g.Where(x => x.Type == EnumTransactionType.Income).Sum(x => x.Amount),
                TotalExpense = g.Where(x => x.Type == EnumTransactionType.Expense).Sum(x => x.Amount)
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync(ct);


        return result.Select(x => new MonthlyTrendResponseModel
        {
            Month = $"{x.Year}-{x.Month:D2}",
            TotalIncome = x.TotalIncome,
            TotalExpense = x.TotalExpense
        }).ToList();
    }

}

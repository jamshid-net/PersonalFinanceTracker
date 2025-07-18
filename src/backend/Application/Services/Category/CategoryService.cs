using FiTrack.Application.Interfaces.Auth;
using FiTrack.Application.Interfaces.Category;
using FiTrack.Application.Interfaces.InfrastructureAdapters;
using FiTrack.Application.Models.Category;
using FiTrack.Application.QueryFilter;
using FiTrack.Domain.Entities.Business;
using FiTrack.Domain.Enums;
using FiTrack.Domain.ValueObjects;

namespace FiTrack.Application.Services.Category;

public class CategoryService(ICurrentUser currentUser,
                             IApplicationDbContext dbContext,
                             IMapper mapper) : ICategoryService
{
    public async Task<CategoryResponseModel> CreateAsync(CategoryRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        var userId = currentUser.Id;
        if (userId is null or 0)
            throw new UnauthorizedAccessException("User is not authenticated");

        var category = new FinCategory
        {
            Name = requestModel.Name,
            UserId = (int)userId,
            Colour = Colour.From(requestModel.ColorCode),
            IsActive = true
        };

        var entityEntry = dbContext.Categories.Add(category);
        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<CategoryResponseModel>(entityEntry.Entity);
    }

    public async Task<CategoryResponseModel> UpdateAsync(CategoryRequestModel requestModel, CancellationToken cancellationToken = default)
    {
        if (requestModel.Id is null or 0)
            throw new ValidationException("Id is required for update");

        var userId = currentUser.Id;
        if (userId is 0)
            throw new UnauthorizedAccessException("User is not authenticated");

        var foundCategory = await dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == requestModel.Id && c.UserId == userId && c.IsActive, cancellationToken);

        if (foundCategory is null)
            throw new NotFoundException(requestModel.Id.ToString()!, nameof(Category));

        foundCategory.Name = requestModel.Name;

        var entityEntry = dbContext.Categories.Update(foundCategory);
        await dbContext.SaveChangesAsync(cancellationToken);

        return mapper.Map<CategoryResponseModel>(entityEntry.Entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var userId = currentUser.Id;
        if (userId == 0)
            throw new UnauthorizedAccessException("User is not authenticated");

        var category = await dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId && c.IsActive, cancellationToken);

        if (category is null)
            throw new NotFoundException(id.ToString(), nameof(Category));

        category.IsActive = false;
        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<CategoryResponseModel> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var userId = currentUser.Id;
        if (userId == 0)
            throw new UnauthorizedAccessException("User is not authenticated");

        var category = await dbContext.Categories
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId && c.IsActive, cancellationToken);

        if (category is null)
            throw new NotFoundException(id.ToString(), nameof(Category));

        return mapper.Map<CategoryResponseModel>(category);
    }

    public Task<PageList<CategoryResponseModel>> GetAllAsync(FilterRequest filterRequest, CancellationToken cancellationToken = default)
    {
        var userId = currentUser.Id;
        if (userId == 0)
            throw new UnauthorizedAccessException("User is not authenticated");

        var query = dbContext.Categories
            .Where(c => c.UserId == userId);

        return query.ProjectTo<CategoryResponseModel>(mapper.ConfigurationProvider)
                    .ToPageListAsync(filterRequest, cancellationToken);
    }

    public async Task<List<TopCategoryExpenseResponseModel>> GetTopExpenseCategoriesAsync(int top = 5, CancellationToken ct = default)
    {
        var userId = currentUser.Id;
        return await dbContext.FinTransactions
            .Where(t => t.UserId == userId && t.Type == EnumTransactionType.Expense)
            .GroupBy(t => t.Category!.Name)
            .Select(g => new TopCategoryExpenseResponseModel
            {
                CategoryName = g.Key,
                TotalExpense = g.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.TotalExpense)
            .Take(top)
            .ToListAsync(ct);
    }

}


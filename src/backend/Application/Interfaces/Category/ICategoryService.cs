using FiTrack.Application.Models.Category;
using FiTrack.Application.QueryFilter;

namespace FiTrack.Application.Interfaces.Category;
public interface ICategoryService
{
    Task<CategoryResponseModel> CreateAsync(CategoryRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<CategoryResponseModel> UpdateAsync(CategoryRequestModel requestModel, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<CategoryResponseModel> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PageList<CategoryResponseModel>> GetAllAsync(FilterRequest filterRequest, CancellationToken cancellationToken = default);
    Task<List<TopCategoryExpenseResponseModel>> GetTopExpenseCategoriesAsync(int top = 5, CancellationToken ct = default);
}

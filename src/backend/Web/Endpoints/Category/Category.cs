using FiTrack.Application.Interfaces.Category;
using FiTrack.Application.Models.Category;
using FiTrack.Application.QueryFilter;
using Microsoft.AspNetCore.Mvc;
using Shared.Helpers;

namespace Web.Endpoints.Category;

public class Category : EndpointGroupBase
{
    public override void Map(RouteGroupBuilder group)
    {
        group.MapPost(CreateCategory).RequireAuthorization();
        group.MapPut(UpdateCategory).RequireAuthorization();
        group.MapDelete(DeleteCategory).RequireAuthorization();
        group.MapGet(GetCategoryById).RequireAuthorization();
        group.MapPost(GetCategories).RequireAuthorization();

        group.MapGet(GetTopExpenseCategories).RequireAuthorization();

    }

    public async Task<ResponseData<CategoryResponseModel>> CreateCategory(
        ICategoryService service,
        [FromBody] CategoryRequestModel requestModel,
        CancellationToken ct)
    {
        return await service.CreateAsync(requestModel, ct);
    }

    public async Task<ResponseData<CategoryResponseModel>> UpdateCategory(
        ICategoryService service,
        [FromBody] CategoryRequestModel requestModel,
        CancellationToken ct)
    {
        return await service.UpdateAsync(requestModel, ct);
    }

    public async Task<ResponseData<bool>> DeleteCategory(
        ICategoryService service,
        int id,
        CancellationToken ct)
    {
        return await service.DeleteAsync(id, ct);
    }

    public async Task<ResponseData<CategoryResponseModel>> GetCategoryById(
        ICategoryService service,
        int id,
        CancellationToken ct)
    {
        return await service.GetByIdAsync(id, ct);
    }

    public async Task<ResponseData<PageList<CategoryResponseModel>>> GetCategories(
        ICategoryService service,
        [FromBody] FilterRequest filter,
        CancellationToken ct)
    {
        return await service.GetAllAsync(filter, ct);
    } 
    public async Task<ResponseData<List<TopCategoryExpenseResponseModel>>> GetTopExpenseCategories(
        ICategoryService service,
        int top,
        CancellationToken ct)
    {
        return await service.GetTopExpenseCategoriesAsync(top, ct);
    }
}

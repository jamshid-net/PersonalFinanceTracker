using FiTrack.Application.Models.Category;
using FiTrack.Domain.Entities.Business;

namespace FiTrack.Application.Mappings;
public class CategoryMappings : Profile
{
    public CategoryMappings()
    {
        CreateMap<FinCategory, CategoryResponseModel>();
    }
}

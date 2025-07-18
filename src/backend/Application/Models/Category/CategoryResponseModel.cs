using FiTrack.Application.Models.Base;
using FiTrack.Domain.ValueObjects;

namespace FiTrack.Application.Models.Category;
public class CategoryResponseModel : BaseAuditResponseModel
{
    public string Name { get; set; } = null!;
    public Colour Colour { get; set; } = null!;
    public int UserId { get; set; }
}

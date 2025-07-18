using FiTrack.Domain.ValueObjects;

namespace FiTrack.Application.Models.Category;
public class CategoryRequestModel
{
    public int? Id { get; set; }
    public required string Name { get; set; }
    public string ColorCode { get; set; } = Colour.White;
}

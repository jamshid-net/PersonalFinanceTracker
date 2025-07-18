using FiTrack.Domain.ValueObjects;

namespace FiTrack.Application.Models.Category;
public class TopCategoryExpenseResponseModel
{
    public Colour Colour { get; set; } = default!;
    public string CategoryName { get; set; } = default!;
    public decimal TotalExpense { get; set; }
}

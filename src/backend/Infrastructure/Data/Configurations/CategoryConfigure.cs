using FiTrack.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiTrack.Infrastructure.Data.Configurations;
public class CategoryConfigure : IEntityTypeConfiguration<FinCategory>
{
    public void Configure(EntityTypeBuilder<FinCategory> builder)
    {
        builder.HasQueryFilter(e => e.IsActive);
    }
}

using FiTrack.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiTrack.Infrastructure.Data.Configurations;
public class FinanceTransactionConfigure : IEntityTypeConfiguration<FinTransaction>
{
    public void Configure(EntityTypeBuilder<FinTransaction> builder)
    {
        builder.Property(x => x.RowVersion)
               .IsRowVersion()
               .ValueGeneratedOnAddOrUpdate()
               .IsRequired();


        builder.HasQueryFilter(e => e.IsActive);
    }
}

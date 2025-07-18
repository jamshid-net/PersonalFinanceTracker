using FiTrack.Domain.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiTrack.Infrastructure.Data.Configurations;
public class AuditLogConfigure : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.Property(e => e.OldValue)
               .HasColumnType("jsonb");
        
        builder.Property(e => e.NewValue)
               .HasColumnType("jsonb");
    }
}

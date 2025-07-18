using FiTrack.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiTrack.Infrastructure.Data.Configurations;
public class AuthUserConfigure : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> builder)
    {
        builder.HasIndex(x => x.UserName)
               .IsUnique();

        builder.HasQueryFilter(e => e.IsActive);
    }
}

using FiTrack.Domain.Entities.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FiTrack.Infrastructure.Data.Configurations;
public class AuthRefreshTokenConfigure : IEntityTypeConfiguration<AuthUserRefreshToken>
{
    public void Configure(EntityTypeBuilder<AuthUserRefreshToken> builder)
    {
        builder.HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .IsRequired(false);
    }
}

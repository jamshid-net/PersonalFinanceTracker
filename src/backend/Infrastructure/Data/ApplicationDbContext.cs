using System.Reflection;
using FiTrack.Application.Interfaces.InfrastructureAdapters;
using FiTrack.Domain.Common;
using FiTrack.Domain.Entities.Audit;
using FiTrack.Domain.Entities.Auth;
using FiTrack.Domain.Entities.Business;
using Microsoft.EntityFrameworkCore;

namespace FiTrack.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options), IApplicationDbContext
{
    #region Auth
    public DbSet<AuthUser> AuthUsers => Set<AuthUser>();
    public DbSet<AuthRole> AuthRoles => Set<AuthRole>();
    public DbSet<AuthPermission> AuthPermissions => Set<AuthPermission>();
    public DbSet<AuthUserRefreshToken> AuthUserRefreshTokens => Set<AuthUserRefreshToken>();

    #endregion

    #region Business
    public DbSet<FinCategory> Categories => Set<FinCategory>();
    public DbSet<FinTransaction> FinTransactions => Set<FinTransaction>();

    #endregion

    #region AuditLog
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    #endregion

    public IQueryable<TEntity> SetEntity<TEntity>() where TEntity : BaseEntity => Set<TEntity>();
    public IQueryable<TEntity> SetEntityNoTracking<TEntity>() where TEntity : class => Set<TEntity>().AsNoTracking();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}

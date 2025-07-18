using FiTrack.Domain.Common;
using FiTrack.Domain.Entities.Audit;
using FiTrack.Domain.Entities.Auth;
using FiTrack.Domain.Entities.Business;

namespace FiTrack.Application.Interfaces.InfrastructureAdapters;

public interface IApplicationDbContext
{
    #region Auth
    DbSet<AuthUser> AuthUsers { get; }
    DbSet<AuthRole> AuthRoles { get; }
    DbSet<AuthPermission> AuthPermissions { get; }
    DbSet<AuthUserRefreshToken> AuthUserRefreshTokens { get; }
    #endregion

    #region Business
    DbSet<FinCategory> Categories { get; }
    DbSet<FinTransaction> FinTransactions { get; }

    #endregion

    #region AuditLog
    DbSet<AuditLog> AuditLogs { get; }
    #endregion
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken);
    IQueryable<TEntity> SetEntity<TEntity>() where TEntity : BaseEntity;
    IQueryable<TEntity> SetEntityNoTracking<TEntity>() where TEntity : class;
}

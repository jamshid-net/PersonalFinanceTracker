using System;
using System.Text.Json;
using FiTrack.Application.Interfaces.Auth;
using FiTrack.Domain.Common;
using FiTrack.Domain.Entities.Audit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FiTrack.Infrastructure.Data.Interceptors;
public class AuditLogInterceptor(
    ICurrentUser currentUser,
    TimeProvider dateTime) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        AddAuditLogs(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void AddAuditLogs(DbContext? context)
    {
        if (context is null) return;
        var userId = currentUser.Id;
        var auditLogs = new List<AuditLog>();
        var utcNow = dateTime.GetUtcNow();
        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>()
                     .Where(e => e.Entity is not AuditLog &&
                                 e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            var entityName = entry.Entity.GetType().Name;

            var entityId = entry.Entity.Id;

            var log = new AuditLog
            {

                UserId = userId,
                Action = entry.State.ToString(),
                EntityName = entityName,
                EntityId = entityId,
                Created = utcNow,
                CreatedBy = userId,
                LastModifiedBy = userId,
                LastModified = utcNow

            };

            if (entry.State == EntityState.Modified)
            {
                log.OldValue = JsonSerializer.Serialize(
                    entry.Properties
                        .Where(p => p.IsModified)
                        .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue));

                log.NewValue = JsonSerializer.Serialize(
                    entry.Properties
                        .Where(p => p.IsModified)
                        .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue));
            }
            else if (entry.State == EntityState.Added)
            {
                log.NewValue = JsonSerializer.Serialize(
                    entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue));
            }
            else if (entry.State == EntityState.Deleted)
            {
                log.OldValue = JsonSerializer.Serialize(
                    entry.Properties.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue));
            }

            auditLogs.Add(log);
        }

        if (auditLogs.Count > 0)
        {
            context.Set<AuditLog>().AddRange(auditLogs);
        }
    }
}


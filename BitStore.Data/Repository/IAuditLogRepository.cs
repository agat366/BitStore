using BitStore.Data.Entities;

namespace BitStore.Data.Repository;

/// <summary>
/// Defines operations for managing order book snapshots data in the database.
/// </summary>
public interface IAuditLogRepository
{
    Task<AuditLog> CreateAsync(AuditLog auditLog);
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId);
}
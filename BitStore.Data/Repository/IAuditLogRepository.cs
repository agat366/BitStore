using BitStore.Data.Entities;

namespace BitStore.Data.Repository;

/// <summary>
/// Defines operations for managing audit log data in the database.
/// </summary>
public interface IAuditLogRepository
{
    Task<AuditLog> CreateAsync(AuditLog auditLog);
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId);
}
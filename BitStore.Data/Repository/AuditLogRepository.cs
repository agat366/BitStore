using BitStore.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitStore.Data.Repository;

/// <summary>
/// Implements audit log data persistence operations using Entity Framework Core.
/// </summary>
public class AuditLogRepository : IAuditLogRepository
{
    private readonly BitStoreDbContext _context;

    public AuditLogRepository(BitStoreDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog> CreateAsync(AuditLog auditLog)
    {
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
        return auditLog;
    }

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId)
    {
        return await _context.AuditLogs
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }
}
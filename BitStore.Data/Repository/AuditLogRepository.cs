using BitStore.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitStore.Data.Repository;

/// <inheritdoc cref="IAuditLogRepository" />
public class AuditLogRepository(BitStoreDbContext context) : IAuditLogRepository
{
    private readonly BitStoreDbContext _context = context;

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
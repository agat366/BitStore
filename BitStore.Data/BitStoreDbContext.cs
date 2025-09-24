using BitStore.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BitStore.Data;

/// <summary>
/// Entity Framework DbContext for BitStore database operations.
/// </summary>
public class BitStoreDbContext(DbContextOptions<BitStoreDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Login).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Token).HasMaxLength(1000);

            entity.HasMany(e => e.AuditLogs)
                  .WithOne(e => e.User)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Timestamp).IsRequired();
            entity.Property(e => e.Data).IsRequired().HasColumnType("nvarchar(max)");
        });
    }
}
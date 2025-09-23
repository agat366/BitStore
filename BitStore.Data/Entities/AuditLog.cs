using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitStore.Data.Entities;

/// <summary>
/// Represents a snapshot of the orders book at the moment of the data request from the user.
/// </summary>
public class AuditLog
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;

    [Required]
    public DateTimeOffset Timestamp { get; set; }

    [Required]
    [Column(TypeName = "nvarchar(max)")]
    public required string Data { get; set; }
}
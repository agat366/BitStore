using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitStore.Data.Entities;

/// <summary>
/// Represents a user entity in the BitStore system.
/// This class is the primary user record storing authentication and identification information.
/// </summary>
public class User
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [MaxLength(100)]
    public required string Login { get; set; }
    
    [MaxLength(1000)]
    public string? Token { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("accounts")]
public class Account
{
  [Key, MaxLength(10)]
  public string Username { get; set; } = string.Empty;

  [Required, MaxLength(20)]
  public string AccessRole { get; set; } = AccessRoles.Viewer;
  
  [Required, MaxLength(255)]
  public string PasswordHash { get; set; } = string.Empty;

  public DateTimeOffset LastLogin { get; set; } = DateTimeOffset.UtcNow;

  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

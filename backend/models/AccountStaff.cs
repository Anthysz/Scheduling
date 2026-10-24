using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("account_staff")]
public class AccountStaff
{
  [Key, MaxLength(10)]
  public string Username { get; set; } = string.Empty;

  // Must stay "Staff"; the database checks it against accounts.access_role.
  [Required, MaxLength(20)]
  public string AccessRole { get; set; } = AccessRoles.Staff;

  [Required, MaxLength(10)]
  public string StaffId { get; set; } = string.Empty;

  [ForeignKey(nameof(Username))]
  public Account Account { get; set; } = null!;

  [ForeignKey(nameof(StaffId))]
  public Staff Staff { get; set; } = null!;
}

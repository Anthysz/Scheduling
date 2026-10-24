using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models;

[Table("staff")]
public class Staff
{
  [Key, MaxLength(10)]
  public string Id { get; set; } = string.Empty;

  [Required, MaxLength(100)]
  public string Name { get; set; } = string.Empty;

  public DateOnly JoinDate { get; set; }

  public DateOnly Birthday { get; set; }

  // Coordinator, Logistics, Consumption, Venue, Manager, ...
  [Required, MaxLength(30)]
  public string JobRole { get; set; } = "Staff";
}
